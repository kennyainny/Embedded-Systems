// * This library is revised by Kehinde Aina, November 2016

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace org.mbed.RPC
{
    // *  This class is used to map a .NET class on to the mbed API C++ class for PwmOut.
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
    public class RPCVariable<T>
    {
        private SerialRPC mbedRPC;
        private String name;

        // * This constructor creates an instance which will control a ValuePass object which has already been created on the mbed
        // * <br>
        // * Note that it must also have the same type as the data on mbed or else data will be parsed incorrectly
        // * 
        // * @param connectedMbed The mbed object for the mbed that the ValuePass has been created on.
        // * @param ExistName The name of the existing ValuePass object
        public RPCVariable(SerialRPC connectedMbed, String ExistName)
        {
            //Tie to existing instance
            mbedRPC = connectedMbed;
            name = ExistName;
        }

        // * Write to a RPCVariable
        // * @param value The value that is to be written.
        public string write(T value)
        {
            String s = value.ToString();
            String[] Args = { s };
            return mbedRPC.RPC(name, "write", Args);
        }

        // * Reads back the current value of the RPCVariable and parses it as a float
        // * @return The value of the RPCVariable as a float
        public float read_float()
        {
            String response = mbedRPC.RPC(name, "read", null);

            //Need to convert response to a float and return
            if (response != null)
            {
                float result = Convert.ToSingle(response);
                return (result);
            }
            else
            {
                Debug.Print("No string was returned to RPCVariable " + name + ". Value set as 0");
                return (0);
            }
        }

        // * Reads back the current value of the RPCVariable and parses it as a String
        // * @return The value of the RPCVariable as a String
        public String read_String()
        {
            String response = mbedRPC.RPC(name, "read", null);

            if (response != null)
            {
                return (response);
            }
            else
            {
                Debug.Print("No string was returned to RPCVariable " + name + " .");
                return (" ");
            }
        }

        // * Reads back the current value of the RPCVariable and parses it as an int
        // * @return The value of the RPCVariable as an int
        public int read_int()
        {
            String response = mbedRPC.RPC(name, "read", null);

            //Need to convert response to an int and return
            if (response != null)
            {
                int result = Convert.ToInt32(response);
                return (result);
            }
            else
            {
                Debug.Print("No string was returned to RPCVariable " + name + " . Value set as 0");
                return (0);
            }
        }

        // * Reads back the current value of the RPCVariable and parses it as a char
        // * @return The value of the RPCVariable as a char
        public char read_char()
        {
            String response = mbedRPC.RPC(name, "read", null);

            //Need to convert response to a char and return
            if (response != null)
            {
                char result = Convert.ToChar(response);     // the first character in the string is converted here
                return (result);
            }
            else
            {
                Debug.Print("No string was returned to RPCVariable " + name + " . Value set as 0");
                return ('0');
            }
        }

        // * Reads back the current value of the RPCVariable and parses it as a double
        // * @return The value of the RPCVariable as a double
        public double read_Double()
        {
            String response = mbedRPC.RPC(name, "read", null);
            //Need to convert response to a float and return

            if (response != null)
            {
                double result = Convert.ToDouble(response);
                return (result);
            }
            else
            {
                Debug.Print("No string was returned to RPCVariable " + name + ". Value set as 0");
                return (0);
            }
        }
    }
}
