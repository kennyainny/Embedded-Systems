// * This library is revised by Kehinde Aina, November 2016

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Mbed.RPC.Serial
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SerialRPCForm());
        }
    }
}


