﻿/* @@@@ HOTTINGER BALDWIN MESSTECHNIK - DARMSTADT @@@@@
 * 
 * TCP/MODBUS Interface for WTX120_Modbus | 01/2018
 * 
 * Author : Felix Huettl
 * 
 *  */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUIplc
{
    /// <summary>
    /// This is the class, which contains the static main method as an entry point into the application. 
    /// </summary>

    static class Program
    {
        /// <summary>
        /// Main entry point for the application
        /// </summary>
        [STAThread]
        static void Main(string[] args) // arguments from the command line of the VS project arguments : IP address and timer interval. 
        {
            Application.EnableVisualStyles(); //This method enables visual styles for the application. Visual styles are the colors, fonts, and other visual elements that form an operating system theme.
            Application.SetCompatibleTextRenderingDefault(false); // This is the standard setting for text rendering. 

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GUIplcForm(args));            
        }
    }
}
