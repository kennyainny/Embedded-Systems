// * This library is revised by Kehinde Aina, November 2016

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace org.mbed.RPC
{
     //*  This class is used to trigger a Function on mbed which is 
     //*  attached to an RPCFunction object.
     //*  This allows you to quickly add RPC to custom bits of code.
     //*  
     //* @author Stanislaus Eichstädt
     //* @license
     //* Copyright (c) 2010 ARM Ltd
     //*
     //*Permission is hereby granted, free of charge, to any person obtaining a copy
     //*of this software and associated documentation files (the "Software"), to deal
     //*in the Software without restriction, including without limitation the rights
     //*to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
     //*copies of the Software, and to permit persons to whom the Software is
     //*furnished to do so, subject to the following conditions:
     //* <br>
     //*The above copyright notice and this permission notice shall be included in
     //*all copies or substantial portions of the Software.
     //* <br>
     //*THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
     //*IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
     //*FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
     //*AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
     //*LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
     //*OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
     //*THE SOFTWARE.
    public class RPCFunction
    {
        private SerialRPC mbedRPC;
        private String name;

        // * @param connectedMbed The mbed this RPCFunction is on
        // * @param ExistName The name of the RPCFunction
        public RPCFunction(SerialRPC connectedMbed, String ExistName)
        {
            //Tie to existing instance
            mbedRPC = connectedMbed;
            name = ExistName;
        }

        // * Read the value of the string output from the atatched function last time it was run.
        // * @return The output from the function as a string
        public String read()
        {
            String response;
            response = mbedRPC.RPC(name, "read", null);
            return (response);
        }

        // * Run the attached function
        // * @param Input The input to be passed into the function
        // * @return The response from the function when it completed
        public String run(String Input)
        {
            ;
            String Output;
            String[] Args = { Input };
            Output = mbedRPC.RPC(name, "run", Args);

            if (Output != null)
            {
                return (Output);
            }
            else
            {
                Debug.Print("No string was returned to RPCFunction " + name + ".");
                return (" ");
            }
        }
    }
}