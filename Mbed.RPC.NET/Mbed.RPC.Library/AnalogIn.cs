// * This library is revised by Kehinde Aina, November 2016

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace org.mbed.RPC
{
    // *  This class is used to map a .NET class on to the mbed API C++ class for AnalogIn.
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
    public class AnalogIn
    {
	    private SerialRPC mbedRPC;
	    private String name;

        // * Create an Analog In on mbed
        // * @param connectedMbed The mbed to create the pin on
        // * @param pin The pin to set as an AnalogIn
		public AnalogIn(SerialRPC connectedMbed, MbedPin pin) 
        {
			//Create a new AnalogIn on mbed
			mbedRPC = connectedMbed;
            name = "mbed_" + pin.PinName.ToLower();
            String[] Args = { pin.PinName, name };
            string parameter = mbedRPC.RPC("AnalogIn", "new", Args);
			Debug.Print("New AnalogIn initialized: " + parameter);            
        }

        //* Tie an existing AnalogIn on mbed to a .NET Object.
        //* @param connectedMbed The mbed the object is on
        //* @param ExistName The name the object has been given on mbed
		public AnalogIn(SerialRPC connectedMbed, String ExistName)
        {
			//Tie to existing instance
			mbedRPC = connectedMbed;
			name = ExistName;
		}

        //* Read the value of the AnalogIn
        //* @return The value of the AnalogIn between 0 and 1
		public float read()
        {
			String response = mbedRPC.RPC(name, "read", null);
            response = response.Replace('.', ',');                  //Hack -> replace '.' to ','!

			//Need to convert response to and int and return
			float i = Convert.ToSingle(response);	
			return(i);
		}

		public void delete()
        {
			mbedRPC.RPC(name, "delete", null);
		}
	}
}
