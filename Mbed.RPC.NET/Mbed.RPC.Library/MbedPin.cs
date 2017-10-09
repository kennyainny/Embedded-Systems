// * This library is revised by Kehinde Aina, November 2016

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace org.mbed.RPC
{
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
    public class MbedPin
    {
        public enum Pins
        {
            LED1, LED2, LED3, LED4,
            p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18,
            p19, p20, p21, p22, p23, p24, p25, p26, p27, p28, p29, p30
        };

        string _pinName;
        //int PinNo;

        // * Create a pin type
        // * @param name The string that should be passed over RPC to mbed to identify this pin ie: "LED1" or "p21"
        public MbedPin(string mbedPinName)
        {
            _pinName = mbedPinName;
        }

        public string PinName
        {
            get
            {
                return _pinName;
            }
        }
    }

    /*
     *  public static PinName LED1 = new PinName("LED1");
        public static PinName LED2 = new PinName("LED2");
        public static PinName LED3 = new PinName("LED3");
        public static PinName LED4 = new PinName("LED4");

        public static PinName p5 = new PinName("p5");
        public static PinName p6 = new PinName("p6");
        public static PinName p7 = new PinName("p7");
        public static PinName p8 = new PinName("p8");
        public static PinName p9 = new PinName("p9");
        public static PinName p10 = new PinName("p10");
        public static PinName p11 = new PinName("p11");
        public static PinName p12 = new PinName("p12");
        public static PinName p13 = new PinName("p13");
        public static PinName p14 = new PinName("p14");
        public static PinName p15 = new PinName("p15");
        public static PinName p16 = new PinName("p16");
        public static PinName p17 = new PinName("p17");
        public static PinName p18 = new PinName("p18");
        public static PinName p19 = new PinName("p19");
        public static PinName p20 = new PinName("p20");
        public static PinName p21 = new PinName("p21");
        public static PinName p22 = new PinName("p22");
        public static PinName p23 = new PinName("p23");
        public static PinName p24 = new PinName("p24");
        public static PinName p25 = new PinName("p25");
        public static PinName p26 = new PinName("p26");
        public static PinName p27 = new PinName("p27");
        public static PinName p28 = new PinName("p28");
        public static PinName p29 = new PinName("p29");
        public static PinName p30 = new PinName("p30");
        */
}
