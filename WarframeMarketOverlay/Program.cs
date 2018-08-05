using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace WarframeMarketOverlay
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

            try
            {
                if (Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1)
                {//Tests if there is already an instance running

                    throw new Exception("There is already an instance of the application running!");
                }

                GlobalHotkeyListener invisibleForm = new GlobalHotkeyListener();
                Application.Run(invisibleForm);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Initialization error");
            }
        }
    }
}
