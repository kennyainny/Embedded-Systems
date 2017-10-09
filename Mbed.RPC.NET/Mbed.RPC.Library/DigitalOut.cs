// * This library is revised by Kehinde Aina, November 2016

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace org.mbed.RPC
{
    // *  This class is used to map a .NET class on to the mbed API C++ class for DigitalOut.
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
    public class DigitalOut
    {
        private SerialRPC mbedRPC;
        private String name;

        // * This is the constructor to create a new Digital Out on mbed
        // * 
        // * @param connectedMbed The mbed object for the mbed that the DigitalOut is to be created on.
        // * @param pin The pin that is to be used as a Digital Out
        public DigitalOut(SerialRPC connectedMbed, MbedPin pin) 
        {
		    //Create a new DigitalOut on mbed
		    mbedRPC = connectedMbed;
            name = "mbed_" + pin.PinName.ToLower();
            String[] Args = {pin.PinName, name};
            string parameter = mbedRPC.RPC("DigitalOut", "new", Args);
            Debug.Print("New DigitalOut initialized: " + parameter);
	    }

        // * This constructor creates an instance which will control a DigitalOut object which has already been created on the mbed
        // * 
        // * @param connectedMbed The mbed object for the mbed that the DigitalOut has been created on.
        // * @param ExistName The name of the existing DigitalOut object
        public DigitalOut(SerialRPC connectedMbed, String ExistName)
        {
            //Tie to existing instance
            mbedRPC = connectedMbed;
            name = ExistName;
            //Debug.Print("Existing DigitalOut pin: " + ExistName);
        }

        // * Write to a digital out
        // * @param value The value that is to be written, either 0 or 1.
        public string write(int value)
        {
            String s = value.ToString();
            String[] Args = { s };
            return mbedRPC.RPC(name, "write", Args);
        }

        // * Reads back the current value of the DigitalOut
        // * @return The value 0 or 1 of the DigitalOut
        public int read()
        {
            String response = mbedRPC.RPC(name, "read", null);
            //Need to convert response to and int and return
            int i = Convert.ToInt32(response);
            return (i);
        }

        public void delete()
        {
            mbedRPC.RPC(name, "delete", null);
        }
    }
}
