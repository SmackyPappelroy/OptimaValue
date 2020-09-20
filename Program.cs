using System;
using System.Threading;
using System.Windows.Forms;

namespace OptimaValue
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool createdNew = true;

            using (Mutex mutex = new Mutex(true, "OptimaValue", out createdNew))
            {
                if (createdNew)
                {
                    Apps.Logger = new FileLogger(@"C:\OptimaValue\");
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new MasterForm());
                }
            }
        }
    }
}
