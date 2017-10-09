// * This library is modified by Kehinde Aina, November 2016

using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace org.mbed.RPC
{
    // *  This class is used to create an object which can communicate using RPC to an mbed Connected over Serial.
    // *  This class requires the .NET Communications API be installed on your computer.
    // * 
    // * @author Stanislaus Eichstädt
    // * @license
    // * Copyright (c) 2010 ARM Ltd
    // *
    // *Permission is hereby granted, free of charge, to any person obtaining a copy
    // *of this software and associated documentation files (the "Software"), to deal
    // *in the Software without restriction, including without limitation the rights
    // *to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    // *copies of the Software, and to permit persons to whom the Software is
    // *furnished to do so, subject to the following conditions:
    // * <br>
    // *The above copyright notice and this permission notice shall be included in
    // *all copies or substantial portions of the Software.
    // * <br>
    // *THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    // *IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    // *FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    // *AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    // *LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    // *OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    // *THE SOFTWARE.
    public class SerialRPC //: MbedPins
    {
	    protected SerialPort mbedSerialPort;
        static String Interrupt;

        // * This creates an mbed object for an mbed connected over Serial.
        // * <br>
        // * Using this class requires the Sun Communications API to be installed
        // * 
        // * @param PortName The Serial Port mbed is connected to eg "COM5" on Windows.
        // * @param Baud The baud rate
        public SerialRPC(String PortName, int Baud) 
        {
            // Create a new SerialPort object with default settings.
            // Open it, too.
            mbedSerialPort = new SerialPort();

            // Set the read/write timeouts
            mbedSerialPort.ReadTimeout = 500;
            mbedSerialPort.WriteTimeout = 500;

            mbedSerialPort.PortName = PortName;
            mbedSerialPort.BaudRate = Baud;

            // always the same
            mbedSerialPort.DataBits = 8;
            mbedSerialPort.Parity = Parity.None;
            mbedSerialPort.StopBits = StopBits.One;

            mbedSerialPort.Open();

            mbedSerialPort.DataReceived += new SerialDataReceivedEventHandler(mbedSerialPort_DataReceived);
            mbedSerialPort.ErrorReceived += new SerialErrorReceivedEventHandler(mbedSerialPort_ErrorReceived);
	    }

	    /**
	     * {@inheritDoc}
	     */
	    public String RPC(String Name, String Method, String[] Args)
        {
		    //write to serial port and receive result
		    String Response = String.Empty;
		    String Arguments = String.Empty;

		    if(Args != null)
            {
			    for(int i = 0; i < Args.Length; i++)
                {
				    Arguments = Arguments + " " + Args[i];
			    }
		    }

            mbedSerialPort.Write("/" + Name + "/" + Method + Arguments + "\n");

		    bool valid = true;

			do
            {
                Response = mbedSerialPort.ReadLine();
			
			    if(Response.Length >= 1)
                {
				    valid = Response.ElementAt(0) != '!';
			    }
			}
            while(valid == false);
            
            //check the return value
            return (Response);
	    }

	    /**
	     * Close the serial port.
	     */
	    public void delete()
        {
		    //Close the serial port
            if (mbedSerialPort != null) mbedSerialPort.Close();
        }

        private void mbedSerialPort_DataReceived(object sender, EventArgs e)
        {
            Debug.Print("Serial Port Data received event");
            Interrupt = mbedSerialPort.ReadLine();
        }

        private void mbedSerialPort_ErrorReceived(object sender, EventArgs e)
        {
            Debug.Print("Serial Port error");
        }
    }
}
