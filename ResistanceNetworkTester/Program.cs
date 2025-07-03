using System;
using System.Windows.Forms;
using ResistanceNetworkTester.UI;

namespace ResistanceNetworkTester
{
    /// <summary>
    /// Main application entry point
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}