using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace vkinviter
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

            Logger.OpenLogFile();
            Logger.LogMethod();

            //Inviter vkinv = new Inviter();
            //vkinv.Login();
            //vkinv.Action();
            //Thread threadVk = new Thread(new ThreadStart(vkinv.invite));
            //threadVk.Start();

            Application.Run(new MainForm());
            Logger.CloseLogFile();
        }
    }
}
