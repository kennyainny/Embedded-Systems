 #include "mbed.h"
#include "SDHCFileSystem.h"
#include "wavfile.h"
#include "Camera_LS_Y201.h"

Serial pc(USBTX, USBRX);
Serial esp(p28, p27); // tx, rx
Serial device(p9, p10);  // tx, rx

//Cam and Mic code
#define DEBMSG      printf
#define NEWLINE()   printf("\r\n")
 
#define USE_SDCARD 1
 
#if USE_SDCARD
#define FILENAME    "/sd/IMG_%04d.jpg"
SDFileSystem fs(p5, p6, p7, p8, "sd");
#else
#define FILENAME    "/local/IMG_%04d.jpg"
LocalFileSystem fs("local");
#endif
//Camera_LS_Y201 cam1(p9, p10);
Camera_LS_Y201 cam1(p13, p14);
////
#define RAM_TOTAL   0x1000

AnalogOut dacout(p18);
AnalogIn adcin(p20);
DigitalOut led_play_ok(LED1);
DigitalOut led_rec_ok(LED2);

Timer timer;
Timer iometer;
Ticker ticker;
SDFileSystem sdc(p5, p6, p7, p8, "sdc");


char ssid[32] = "trojan";     // enter WiFi router ssid inside the quotes
char pwd [32] = "mypass123"; // enter WiFi router password inside the quotes

// Definitions of iRobot Roomba SCI Command Numbers
// See the Roomba SCI manual for a complete list 
//                 Create Command              // Arguments
const char         Start = 128;
const char         Control = 130;
const char         FullMode = 132;
const char         SafeMode = 131;
const char         Drive = 137;                // 4:   [Vel. Hi] [Vel Low] [Rad. Hi] [Rad. Low]
const char         Sensors = 142;              // 1:    Sensor Packet ID
const char         CoverandDock = 143;         // 0:    Return to Charger
const char         Clean = 135;                 // 0:    Start Cleaning
const char         PlaySong = 141;
const char         Song = 140;
                /* iRobot Roomba Sensor IDs */
const char         BumpsandDrops = 1;
 
int speed =  400;
int radius = 0x8000;
void start();
void forward();
void reverse();
void left();
void right();
void stop();
void playsong();
void charger();

// Standard Mbed LED definitions
DigitalOut  led1(LED1),led2(LED2),led3(LED3),led4(LED4);

// things for sending/receiving data over serial
volatile int tx_in=0;
volatile int tx_out=0;
volatile int rx_in=0;
volatile int rx_out=0;
const int buffer_size = 4095;
char tx_buffer[buffer_size+1], rx_buffer[buffer_size+1];
void Tx_interrupt(),Rx_interrupt(),send_line(),read_line();

int roombaDirection =0;

int DataRX, update, count, timeout;
char cmdbuff[1024], replybuff[4096];
char webdata[4096]; // This may need to be bigger depending on WEB browser used
char webbuff[4096];     // Currently using 1986 characters, Increase this if more web page data added
char timebuf[30];
void SendCMD(),getreply(),ReadWebData(),startserver(),ESPconfig(), moveRobo();

char rx_line[1024];
int port        =80;  // set server port

//-------------------Cam and Mic functions------------------
float buffer[RAM_TOTAL];
int rp = 0;
int wp = 0;
int dropout = 0;

#define WAVFILE_ERROR_PRINT(RESULT) \
    do { \
        WavFileResult R = RESULT; \
        if (R != WavFileResultOK) { \
            char wavfile_error_print_text[BUFSIZ]; \
            wavfile_result_string(R, wavfile_error_print_text, sizeof(wavfile_error_print_text)); \
            printf("%s (code=%d)\r\n", wavfile_error_print_text, R); \
            return 1; \
        } \
    } while(0)

//////
typedef struct work {
    FILE *fp;
} work_t;
 
work_t work;

void callback_func(int done, int total, uint8_t *buf, size_t siz)
{
    fwrite(buf, siz, 1, work.fp);
 
    static int n = 0;
    int tmp = done * 100 / total;
    if (n != tmp) {
        n = tmp;
        DEBMSG("Writing...: %3d%%", n);
        NEWLINE();
    }
}

int capture(Camera_LS_Y201 *cam, char *filename)
{
    /*
     * Take a picture.
     */
    if (cam->takePicture() != 0) {
        return -1;
    }
    DEBMSG("Captured.");
    NEWLINE();
 
    /*
     * Open file.
     */
    work.fp = fopen(filename, "wb");
    if (work.fp == NULL) {
        return -2;
    }
 
    /*
     * Read the content.
     */
    DEBMSG("%s", filename);
    NEWLINE();
    if (cam->readJpegFileContent(callback_func) != 0) {
        fclose(work.fp);
        return -3;
    }
    fclose(work.fp);
 
    /*
     * Stop taking pictures.
     */
    cam->stopTakingPictures();
 
    return 0;
}

void tickplay(void)
{
    /*
     * Check the play underflow
     */
    if (rp != wp) {
        dacout = buffer[rp];
        rp = (rp + 1) & (RAM_TOTAL - 1);
    } else {
        dropout++;
    }
    led_play_ok = !led_play_ok;
}

void tickrec(void)
{
    /*
     * Check the rec overflow
     */
    int np = (wp + 1) & (RAM_TOTAL - 1);
    if (np != rp) {
        buffer[wp] = adcin;
        wp = np;
    } else {
        dropout++;
    }
    led_rec_ok = !led_rec_ok;
}

int play(const char *filename)
{
    WavFileResult result;
    wavfile_info_t info;
    wavfile_data_t data;

    WAVFILE *wf = wavfile_open(filename, WavFileModeRead, &result);
    WAVFILE_ERROR_PRINT(result);
    WAVFILE_ERROR_PRINT(wavfile_read_info(wf, &info));

    printf("[PLAY:%s]\r\n", filename);
    printf("\tWAVFILE_INFO_AUDIO_FORMAT(&info)    = %d\r\n", WAVFILE_INFO_AUDIO_FORMAT(&info));
    printf("\tWAVFILE_INFO_NUM_CHANNELS(&info)    = %d\r\n", WAVFILE_INFO_NUM_CHANNELS(&info));
    printf("\tWAVFILE_INFO_SAMPLE_RATE(&info)     = %d\r\n", WAVFILE_INFO_SAMPLE_RATE(&info));
    printf("\tWAVFILE_INFO_BYTE_RATE(&info)       = %d\r\n", WAVFILE_INFO_BYTE_RATE(&info));
    printf("\tWAVFILE_INFO_BLOCK_ALIGN(&info)     = %d\r\n", WAVFILE_INFO_BLOCK_ALIGN(&info));
    printf("\tWAVFILE_INFO_BITS_PER_SAMPLE(&info) = %d\r\n", WAVFILE_INFO_BITS_PER_SAMPLE(&info));

    const int interval_us =  1000000 / WAVFILE_INFO_SAMPLE_RATE(&info);

    rp = 0;
    wp = 0;
    dropout = 0;
    ticker.attach_us(tickplay, interval_us);
    while (1) {
        int np = (wp + 1) & (RAM_TOTAL - 1);
        while (np == rp) {
            wait_us(1);
        }
        WAVFILE_ERROR_PRINT(wavfile_read_data(wf, &data));
        if (WAVFILE_DATA_IS_END_OF_DATA(&data)) {
            break;
        }
        buffer[wp] = WAVFILE_DATA_CHANNEL_DATA(&data, 0);
        wp = np;
    }
    ticker.detach();
    dacout = 0.5;
    led_play_ok = 0;
    printf("\t-- Play done. (dropout=%d) --\r\n", dropout);

    WAVFILE_ERROR_PRINT(wavfile_close(wf));
    return 0;
}

int rec(const char *filename, const int nsec)
{
    WavFileResult result;
    wavfile_info_t info;
    wavfile_data_t data;

    WAVFILE_INFO_AUDIO_FORMAT(&info)    = 1;
    WAVFILE_INFO_NUM_CHANNELS(&info)    = 1;
    WAVFILE_INFO_SAMPLE_RATE(&info)     = 11025;
    WAVFILE_INFO_BYTE_RATE(&info)       = 22050;
    WAVFILE_INFO_BLOCK_ALIGN(&info)     = 2;
    WAVFILE_INFO_BITS_PER_SAMPLE(&info) = 16;

    WAVFILE *wf = wavfile_open(filename, WavFileModeWrite, &result);
    WAVFILE_ERROR_PRINT(result);
    WAVFILE_ERROR_PRINT(wavfile_write_info(wf, &info));

    printf("[REC:%s]\r\n", filename);
    printf("\tWAVFILE_INFO_AUDIO_FORMAT(&info)    = %d\r\n", WAVFILE_INFO_AUDIO_FORMAT(&info));
    printf("\tWAVFILE_INFO_NUM_CHANNELS(&info)    = %d\r\n", WAVFILE_INFO_NUM_CHANNELS(&info));
    printf("\tWAVFILE_INFO_SAMPLE_RATE(&info)     = %d\r\n", WAVFILE_INFO_SAMPLE_RATE(&info));
    printf("\tWAVFILE_INFO_BYTE_RATE(&info)       = %d\r\n", WAVFILE_INFO_BYTE_RATE(&info));
    printf("\tWAVFILE_INFO_BLOCK_ALIGN(&info)     = %d\r\n", WAVFILE_INFO_BLOCK_ALIGN(&info));
    printf("\tWAVFILE_INFO_BITS_PER_SAMPLE(&info) = %d\r\n", WAVFILE_INFO_BITS_PER_SAMPLE(&info));

    const int interval_us =  1000000 / WAVFILE_INFO_SAMPLE_RATE(&info);
    const unsigned int samples_for_nsec = WAVFILE_INFO_SAMPLE_RATE(&info) * nsec;

    rp = 0;
    wp = 0;
    dropout = 0;
    unsigned int count = 0;
    ticker.attach_us(tickrec, interval_us);
    WAVFILE_DATA_NUM_CHANNELS(&data) = 1;
    while (1) {
        while (rp == wp) {
            wait_us(1);
        }

        WAVFILE_DATA_CHANNEL_DATA(&data, 0) = buffer[rp];
        rp = (rp + 1) & (RAM_TOTAL - 1);
        WAVFILE_ERROR_PRINT(wavfile_write_data(wf, &data));

        count++;
        if (count > samples_for_nsec) {
            break;
        }
    }
    ticker.detach();
    led_rec_ok = 0;
    printf("\t-- Rec done. (dropout=%d) --\r\n", dropout);

    WAVFILE_ERROR_PRINT(wavfile_close(wf));
    return 0;
}
//-------------------------Cam and Mic ends here-----------------

int main()
{
    device.baud(57600);   
    start();
    playsong();
    
    for(int i=0;i<10;i++)
    {
            led1=!led1;wait(0.2);led1=!led1;led2=!led2;
            wait(0.2);led2=!led2;led3=!led3;wait(0.2);
            led3=!led3;led4=!led4;wait(0.2);led4=!led4;            
    }
    
    pc.baud(9600);
    esp.baud(9600);
    led1=0,led2=0,led3=0, led4=0;
    
    timeout=2;
    pc.printf("Welcome!\r\n");
    
    esp.attach(&Rx_interrupt, Serial::RxIrq);     // Setup a serial interrupt function to receive data
    esp.attach(&Tx_interrupt, Serial::TxIrq);    // Setup a serial interrupt function to transmit data
    
    startserver();
    
    DataRX=0;
    count=0;
 
    while(1) {
        if(DataRX==1) {
            ReadWebData();
            esp.attach(&Rx_interrupt, Serial::RxIrq);
        }
        if(update==1) // update time, hit count, and analog levels in the HUZZAH chip
        {
            moveRobo();
            count++;
            update=0;   
        }
    }
}

// Reads and processes GET and POST web data
void ReadWebData()
{
    wait_ms(200);
    esp.attach(NULL,Serial::RxIrq);
    DataRX=0;
    memset(webdata, '\0', sizeof(webdata));
    strcpy(webdata, rx_buffer);
    memset(rx_buffer, '\0', sizeof(rx_buffer));
    rx_in = 0;
    rx_out = 0;
    // check web data for form information
    if( strstr(webdata, "check=led1v") != NULL ) {
        //led1=!led1;
        roombaDirection = 1;
    }
    if( strstr(webdata, "check=led2v") != NULL ) {
        //led2=!led2;
        roombaDirection =2;
    }
    if( strstr(webdata, "check=led3v") != NULL ) {
        //led3=!led3;
        roombaDirection =3;
    }
    if( strstr(webdata, "check=led4v") != NULL ) {
        //led4=!led4;
        roombaDirection =4;
    }
    if( strstr(webdata, "check=led5v") != NULL ) {
        //led4=!led4;
        roombaDirection =5;
    }
    if((strstr(webdata, "POST")!= NULL ) || ( strstr(webdata, "GET") != NULL)) { // set update flag if POST request
        update=1;
    }
}

void moveRobo()
{
  switch (roombaDirection)
    {
      case 1:
      {         
        forward();
        wait(3);
        stop();
        roombaDirection =0;
        pc.printf("Forward\r\n");
        break;
      }
      case 2:
      {
        reverse();
        wait(3);
        stop();
        roombaDirection =0;
        pc.printf("Backward\r\n");
        break;
       }
      case 3:
      {
        left();
        wait(0.5);
        stop();
        wait(0.1);        
        forward();
        wait(2);
        stop();
        roombaDirection =0;
        pc.printf("Left\r\n");
        break;
      }
      case 4:
      {
        right();        
        wait(0.5);
        stop();
        wait(0.1);        
        forward();
        wait(2);                
        stop();
        roombaDirection =0;
        pc.printf("Right\r\n");
        break;
      }
      case 5:
      {
        static const char *target_filename = "/sdc/rec-test1.wav";
        
        while (1) 
        {
            /*
             * 30 seconds recording.
             */
            if (rec(target_filename, 30) != 0) {
                break;
            }
            WavFileResult result;
            wavfile_info_t info;
            wavfile_data_t data;
            /*
             * Play it!
             */
            //if (play(target_filename) != 0) {
                break;
            //}
        
        }
        ////
        DEBMSG("Camera module");
        NEWLINE();
        DEBMSG("Resetting...");
        NEWLINE();
        wait(1);
     
        if (cam1.reset() == 0) {
            DEBMSG("Reset OK.");
            NEWLINE();
        } else {
            DEBMSG("Reset fail.");
            NEWLINE();
            error("Reset fail.");
        }
        wait(1);
     
        char fname[64];
        snprintf(fname, sizeof(fname) - 1, FILENAME, 1);
        int r = capture(&cam1, fname);
        if (r == 0) 
        {
            DEBMSG("[%04d]:OK.", 1);
            NEWLINE();
        } 
        else 
        {
            DEBMSG("[%04d]:NG. (code=%d)", 1, r);
            NEWLINE();
            error("Failure.");
        }
       
      }      
    }
} 

// Starts webserver
void startserver()
{    
    pc.printf("\n++++++++++ Setting up HTTP Server ++++++++++\r\n> ");
    
    strcpy(cmdbuff, "ip, nm, gw=wifi.sta.getip()\r\n");
    SendCMD();
    getreply();
    wait(0.2);
    strcpy(cmdbuff,"print(\"IP Address: \",ip)\r\n");
    SendCMD();
    getreply();
    wait(0.2);
 
    //create server
    sprintf(cmdbuff, "srv=net.createServer(net.TCP)\r\n");
    SendCMD();
    getreply();
    wait(0.5);
    strcpy(cmdbuff,"srv:listen(80,function(conn)\r\n");
    SendCMD();
    getreply();
    wait(0.3);
        strcpy(cmdbuff,"conn:on(\"receive\",function(conn,payload) \r\n");
        SendCMD();
        getreply();
        wait(0.3);
        
        //print data to mbed
        strcpy(cmdbuff,"print(payload)\r\n");
        SendCMD();
        getreply();
        wait(0.2);
       
        //web page data
        strcpy(cmdbuff,"conn:send('<!DOCTYPE html><html><body><h1>IoT Controlled Spy Robot</h1>')\r\n");
        SendCMD();
        getreply();
        wait(0.4);
        strcpy(cmdbuff,"conn:send('<form method=\"POST\"')\r\n");
        SendCMD();
        getreply();
        wait(0.3);
        strcpy(cmdbuff, "conn:send('<p><input type=\"checkbox\" name=\"check\" value=\"led1v\"> Forward</p>')\r\n");
        SendCMD();
        getreply();
        wait(0.3);
        strcpy(cmdbuff, "conn:send('<p><input type=\"checkbox\" name=\"check\" value=\"led2v\"> Backward</p>')\r\n");
        SendCMD();
        getreply();
        wait(0.3);
        strcpy(cmdbuff, "conn:send('<p><input type=\"checkbox\" name=\"check\" value=\"led3v\"> Left</p>')\r\n");
        SendCMD();
        getreply();
        wait(0.3);
        strcpy(cmdbuff, "conn:send('<p><input type=\"checkbox\" name=\"check\" value=\"led4v\"> Right</p>')\r\n");
        SendCMD();
        getreply();
        wait(0.3);
        strcpy(cmdbuff, "conn:send('<p><input type=\"checkbox\" name=\"check\" value=\"led5v\"> Snap-Rec</p>')\r\n");
        SendCMD();
        getreply();
        wait(0.3);        
        strcpy(cmdbuff,"conn:send('<p><input type=\"submit\" value=\"SEND\"></p></form>')\r\n");
        SendCMD();
        getreply();
        wait(0.3);
        
        strcpy(cmdbuff,"conn:send('<h2>Camera Image</h2>')\r\n");
        SendCMD();
        getreply();
        wait(0.3);
        strcpy(cmdbuff,"conn:send('<img src=\"IMG_0001.jpg\" alt=\"cam-img\" style=\"width:320px;height:240px;\">')\r\n");
        SendCMD();
        getreply();
        wait(0.3);
        
        strcpy(cmdbuff, "conn:send('<p><h2>Tip: </h2></p><ul><li>Select a checkbox to specify direction</li><li>Click SEND to send data to robot</li></ul></body></html>')\r\n");
        SendCMD();
        getreply();
        wait(0.5); 
        // end web page data
        strcpy(cmdbuff, "conn:on(\"sent\",function(conn) conn:close() end)\r\n"); // close current connection
        SendCMD();
        getreply();
        wait(0.3);
        strcpy(cmdbuff, "end)\r\n");
        SendCMD();
        getreply();
        wait(0.2);
    strcpy(cmdbuff, "end)\r\n");
    SendCMD();
    getreply();
    wait(0.2);
    
    pc.printf("\n\n++++++++++ Ready ++++++++++\r\n\n");
}

// ESP Command data send
void SendCMD()
{
    int i;
    char temp_char;
    bool empty;
    i = 0;
// Start Critical Section - don't interrupt while changing global buffer variables
    NVIC_DisableIRQ(UART1_IRQn);
    empty = (tx_in == tx_out);
    while ((i==0) || (cmdbuff[i-1] != '\n')) {
// Wait if buffer full
        if (((tx_in + 1) % buffer_size) == tx_out) {
// End Critical Section - need to let interrupt routine empty buffer by sending
            NVIC_EnableIRQ(UART1_IRQn);
            while (((tx_in + 1) % buffer_size) == tx_out) {
            }
// Start Critical Section - don't interrupt while changing global buffer variables
            NVIC_DisableIRQ(UART1_IRQn);
        }
        tx_buffer[tx_in] = cmdbuff[i];
        i++;
        tx_in = (tx_in + 1) % buffer_size;
    }
    if (esp.writeable() && (empty)) {
        temp_char = tx_buffer[tx_out];
        tx_out = (tx_out + 1) % buffer_size;
// Send first character to start tx interrupts, if stopped
        esp.putc(temp_char);
    }
// End Critical Section
    NVIC_EnableIRQ(UART1_IRQn);
    return;
}

// Get Command and ESP status replies
void getreply()
{
    read_line();
    sscanf(rx_line,replybuff);
}
 
// Read a line from the large rx buffer from rx interrupt routine
void read_line() {
    int i;
    i = 0;
// Start Critical Section - don't interrupt while changing global buffer variables
    NVIC_DisableIRQ(UART1_IRQn);
// Loop reading rx buffer characters until end of line character
    while ((i==0) || (rx_line[i-1] != '\r')) {
// Wait if buffer empty
        if (rx_in == rx_out) {
// End Critical Section - need to allow rx interrupt to get new characters for buffer
            NVIC_EnableIRQ(UART1_IRQn);
            while (rx_in == rx_out) {
            }
// Start Critical Section - don't interrupt while changing global buffer variables
            NVIC_DisableIRQ(UART1_IRQn);
        }
        rx_line[i] = rx_buffer[rx_out];
        i++;
        rx_out = (rx_out + 1) % buffer_size;
    }
// End Critical Section
    NVIC_EnableIRQ(UART1_IRQn);
    rx_line[i-1] = 0;
    return;
}
  
// Interupt Routine to read in data from serial port
void Rx_interrupt() {
    DataRX=1;
    //led3=1;
// Loop just in case more than one character is in UART's receive FIFO buffer
// Stop if buffer full
    while ((esp.readable()) && (((rx_in + 1) % buffer_size) != rx_out)) {
        rx_buffer[rx_in] = esp.getc();
// Uncomment to Echo to USB serial to watch data flow
        pc.putc(rx_buffer[rx_in]);
        rx_in = (rx_in + 1) % buffer_size;
    }
    //led3=0;
    return;
}
 
// Interupt Routine to write out data to serial port
void Tx_interrupt() {
    //led2=1;
// Loop to fill more than one character in UART's transmit FIFO buffer
// Stop if buffer empty
    while ((esp.writeable()) && (tx_in != tx_out)) {
        esp.putc(tx_buffer[tx_out]);
        tx_out = (tx_out + 1) % buffer_size;
    }
    //led2=0;
    return;
}

 


//--------------------Roomba section----------------------------------
 
// Start  - send start and safe mode, start streaming sensor data
void start() {
   // device.printf("%c%c", Start, SafeMode);
    device.putc(Start);
    
    //device.putc(FullMode);

    wait(.1);
    device.putc(Control);
    wait(.5);
  //  device.printf("%c%c", SensorStream, char(1));
    device.putc(Sensors);
    device.putc(BumpsandDrops);
    wait(.5);
}
// Stop  - turn off drive motors
void stop() {
    device.printf("%c%c%c%c%c", Drive, char(0),  char(0),  char(0),  char(0));
}
// Forward  - turn on drive motors
void forward() {
    device.printf("%c%c%c%c%c", Drive, char((speed>>8)&0xFF),  char(speed&0xFF),  
    char((radius>>8)&0xFF),  char(radius&0xFF));
 
}
// Reverse - reverse drive motors
void reverse() {
    device.printf("%c%c%c%c%c", Drive, char(((-speed)>>8)&0xFF),  char((-speed)&0xFF),  
    char(((radius)>>8)&0xFF),  char((radius)&0xFF));
 
}
// Left - drive motors set to rotate to left
void left() {
    device.printf("%c%c%c%c%c", Drive, char((speed>>8)&0xFF),  char(speed&0xFF),  
    char(((1)>>8)&0xFF),  char((1)&0xFF));
}
// Right - drive motors set to rotate to right
void right() {
    device.printf("%c%c%c%c%c", Drive, char(((speed)>>8)&0xFF),  char((speed)&0xFF),  
    char((-1>>8)&0xFF),  char(-1&0xFF));
 
}
// Charger - search and return to charger using IR beacons (if found)
void charger() {
    device.printf("%c", Clean );
    wait(.2);
    device.printf("%c", CoverandDock );
}
// Play Song  - define and play a song
void playsong() { // Send out notes & duration to define song and then play song
 
    device.printf("%c%c%c%c%c%c%c%c%c%c%c%c%c%c%c%c%c%c%c%c%c%c%c%c%c%c%c%c%c%c%c%c%c%c%c", 
                  Song, char(0), char(16), char(91), char(24), char(89), char(12), char(87), char(36), char(87),
                  char(24), char(89), char(12), char(91), char(24), char(91), char(12), char(91), char(12), char(89),
                  char(12),char(87), char(12), char(89), char(12), char(91), char(12), char(89), char(12), char(87),
                  char(24), char(86), char(12), char(87), char(48));
 
    wait(.2);
    device.printf("%c%c", PlaySong, char(0));
}
 