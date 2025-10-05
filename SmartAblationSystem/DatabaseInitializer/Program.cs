using System;
using System.Windows.Forms;

namespace DatabaseFiller
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// IEC 62304 Class A.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmDatabaseInitializer());
        }
    }
}