using org.mbed.RPC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Mbed.RPC.Serial
{
    public partial class SerialRPCForm : Form
    {
        DigitalOut _led1, _led2, _led3, _led4;        // DigitalOut RPC Object
        int led1Status, led2Status, led3Status, led4Status, commStatus;        // led status variables
        string selectedPort;
        SerialRPC _serialRPC;    // embed rpc handle

        public SerialRPCForm()
        {
            InitializeComponent();
            //get list of active ports on the computer
            string[] ports = SerialPort.GetPortNames();
            serialComboBox.Items.AddRange(ports);
        }

        private void SerialRPCForm_Load(object sender, EventArgs e)
        {            
            //disable controls until com port is connected
            groupBox1.Enabled = false;
            startButton.Enabled = false;

            //initialize status variables
            commStatus = 0; led1Status = 0; led2Status = 0; led3Status = 0; led4Status = 0;
            statusLabel.Text = "Not connected!";
        }
      
        private void serialComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedPort = serialComboBox.SelectedItem.ToString();

            try
            {
                //Create an mbed object for communication over USB (serial)
                _serialRPC = new SerialRPC(selectedPort, 9600);

                //Create new Digital Outputs on the mbed
                _led1 = new DigitalOut(_serialRPC, new MbedPin("LED1"));
                _led2 = new DigitalOut(_serialRPC, new MbedPin("LED2"));
                _led3 = new DigitalOut(_serialRPC, new MbedPin("LED3"));
                _led4 = new DigitalOut(_serialRPC, new MbedPin("LED4"));

                //enable controls after com port is connected
                groupBox1.Enabled = true;
                startButton.Enabled = true;

                //MessageBox.Show(selectedPort +" connected to Mbed", "Mbed Connected!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                commStatus = 1;
                serialComboBox.Enabled = false;
                statusLabel.Text = "Mbed connected to " + selectedPort;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                commStatus = 0;
                if (_led1 != null) _led1.delete();
                if (_led2 != null) _led2.delete();
                if (_led3 != null) _led3.delete();
                if (_led4 != null) _led4.delete();
                
                if (_serialRPC != null) _serialRPC.delete();
                
                //disable controls if mbed is disconnected
                groupBox1.Enabled = false;
                startButton.Enabled = false;

                statusLabel.Text = "Not connected!";
            }
        }

        //fire an event if when checkbox changes            
        private void led1_chkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (led1_chkBox.Checked)
            {
                led1Status = 1;
                string _response = _led1.write(1);      //Led on
            }
            else
            {
                led1Status = 0;
                string _response = _led1.write(0);        //Led off
            }
        }

        //fire an event if when checkbox changes            
        private void led2_chkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (led2_chkBox.Checked)
            {
                led2Status = 1;
                string _response = _led2.write(1);         //Led on
            }
            else
            {
                led2Status = 0;
                string _response = _led2.write(0);          //Led off
            }
        }

        //fire an event if when checkbox changes            
        private void led3_chkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (led3_chkBox.Checked)
            {
                led3Status = 1;
                string _response = _led3.write(1);          //Led on
            }
            else
            {
                led3Status = 0;
                string _response = _led3.write(0);          //Led off
            }
        }

        //fire an event if when checkbox changes            
        private void led4_chkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (led4_chkBox.Checked)
            {
                led4Status = 1;
                string _response = _led4.write(1);           //Led on
            }
            else
            {
                led4Status = 0;
                string _response = _led4.write(0);          //Led off
            }
        }

        //LED blinky event
        private void button1_Click(object sender, EventArgs e)
        {
            statusLabel.Text = "Busy! - RPC action in progress...";
            groupBox1.Enabled = false;
            startButton.Enabled = false;
            startButton.Enabled = false;
            stopButton.Enabled = false;
            
            BlinkLed(6);

            stopButton.Enabled = true;
            startButton.Enabled = true;
            groupBox1.Enabled = true;
            startButton.Enabled = true;
            statusLabel.Text = "Mbed connected to " + selectedPort;
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            try
            {
                //delete objects before exit
                if (_led1 != null)
                {
                    _led1.write(0);
                    _led1.delete();
                }
                if (_led2 != null)
                {
                    _led2.write(0);
                    _led2.delete();
                }
                if (_led3 != null)
                {
                    _led3.write(0);
                    _led3.delete();
                }
                if (_led4 != null)
                {
                    _led4.write(0);
                    _led4.delete();
                }

                if (_serialRPC != null) _serialRPC.delete();

                Debug.Print("Complete");
            }
            catch (NullReferenceException ex)
            {
                Debug.Print("No Reference: " + ex.Message);
            }
            this.Close();
        }


        //custom functions/methods
        private void BlinkLed(int n)
        {
            try
            {
                for (int i = 0; i < n; i++)
                {
                    led1Status = 1 - led1Status; //flip between 0 and 1
                    _led1.write(led1Status);
                    led2Status = 1 - led2Status;
                    _led2.write(led2Status);          
                    led3Status = 1 - led3Status;
                    _led3.write(led3Status);         
                    led4Status = 1 - led4Status;
                    _led4.write(led4Status);                        
                    Thread.Sleep(250);
                }
            }
            catch (NullReferenceException ex)
            {
                Debug.Print("No Reference: " + ex.Message);
                statusLabel.Text = ex.Message;
            }
        }

    }
}
