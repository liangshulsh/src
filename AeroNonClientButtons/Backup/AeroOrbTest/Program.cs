using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AeroOrbTest;

namespace AeroNonClientButtons
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //int enabled = 0;
            //int response = Dwm.DwmIsCompositionEnabled(ref enabled);

            //if (enabled == 0)
            //{
            //    MessageBox.Show("You must have Aero enabled!");
            //    return;
            //}


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}