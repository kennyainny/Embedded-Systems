// * This library is revised by Kehinde Aina, November 2016

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace org.mbed.RPC
{
    // *  This class is used to map a .NET class on to the mbed API C++ class for Serial.
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
    public class Serial
    {
        private SerialRPC mbedRPC;
        private String name;

        public Serial(SerialRPC connectedMbed, MbedPin TxPin, MbedPin RxPin) 
        {
			//Create a new Serial on mbed
			mbedRPC = connectedMbed;
            name = "serial_" + TxPin.PinName + RxPin.PinName;
            String[] Args = {TxPin.PinName, RxPin.PinName, name};
            string parameter = mbedRPC.RPC("Serial", "new", Args);
			Debug.Print("New Serial object initialized: " + parameter);            
        }

        public Serial(SerialRPC connectedMbed, String ExistName)
        {
            //Tie to existing instance
            mbedRPC = connectedMbed;
            name = ExistName;
        }

        public void baud(int baudRate)
        {
            String[] Args = { baudRate.ToString() };
            mbedRPC.RPC(name, "baud", Args);
        }

        public void format(int bits, String Parity, int stop_bits)
        {
            String[] Args = { bits.ToString(), Parity, stop_bits.ToString() };
            mbedRPC.RPC(name, "format", Args);
        }

        public void putc(char c)
        {
            String[] Args = { Convert.ToString(c) };
            mbedRPC.RPC(name, "putc", Args);
        }

        public char getc()
        {
            String response = mbedRPC.RPC(name, "getc", null);
            char c = Convert.ToChar(response);
            return (c);
        }

        public String gets()
        {
            String rxString = String.Empty;
            int i = 0;
            char c;

            do
            {
                c = getc();

                //rxString.concat(Character.toString(c));
                rxString = rxString + c;
                i++;
            } 
            while (c != (char)10); //line feed

            return (rxString);
        }

        public void puts(String data)
        {
            char c;

            for (int i = 0; i < data.Length; i++)
            {
                c = data.ElementAt(i);
                putc(c);
            }
            putc('\n');
        }

        public bool readable()
        {
            String result = mbedRPC.RPC(name, "readable", null);

            //use correct form depending on how mbed responds
            bool readable = Convert.ToBoolean(result);
            return (readable);
        }

        public bool writeable()
        {
            String result = mbedRPC.RPC(name, "writeable", null);
            bool writeable = Convert.ToBoolean(result);
            return (writeable);
        }

        public void delete()
        {
            mbedRPC.RPC(name, "delete", null);
        }
    }
}
