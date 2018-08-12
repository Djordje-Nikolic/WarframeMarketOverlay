using System;
using System.Windows.Forms;

namespace WarframeMarketOverlay
{
    class WarframeTrayIconException : Exception
    {
        public WarframeTrayIconException() { }

        public WarframeTrayIconException(string message) : base(message) { }

        public WarframeTrayIconException(string message, Exception inner) : base(message, inner) { }
    }

    class WarframeTrayIcon : IDisposable
    {
        private NotifyIcon systemTrayIcon;
        private Form relatedForm;

        public WarframeTrayIcon(Form form)
        {
            systemTrayIcon = null;
            relatedForm = form;
        }

        public void Initialize()
        {//Initializes the system tray icon and it's components

            systemTrayIcon = new NotifyIcon();
            try
            {
                systemTrayIcon.Icon = new System.Drawing.Icon("Warframe Market Overlay.ico");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                systemTrayIcon.Icon = System.Drawing.SystemIcons.WinLogo;
            }
            systemTrayIcon.Text = "Warframe Market Overlay";

            MenuItem donateItem = new MenuItem
            {
                Index = 0,
                Text = "Donate"
            };
            donateItem.Click += new EventHandler(DonateItem_Click);

            MenuItem helpItem = new MenuItem
            {
                Index = 1,
                Text = "Help"
            };
            helpItem.Click += new EventHandler(HelpItem_Click);

            MenuItem optionsItem = new MenuItem
            {
                Index = 2,
                Text = "Options"
            };
            optionsItem.Click += new EventHandler(OptionsItem_Click);

            MenuItem exitItem = new MenuItem
            {
                Index = 3,
                Text = "Exit"
            };
            exitItem.Click += new EventHandler(ExitItem_Click);

            ContextMenu trayContextMenu = new ContextMenu();
            trayContextMenu.MenuItems.AddRange(new MenuItem[] { donateItem, helpItem, optionsItem, exitItem });
            systemTrayIcon.ContextMenu = trayContextMenu;
            SetTextFailure();
            systemTrayIcon.Visible = true;
        }   //called when the form has loaded

        public void SetTextSuccess()
        {
            if (systemTrayIcon != null)
            {
                systemTrayIcon.Text = "Warframe Market Overlay (Warframe is running)";
                systemTrayIcon.ShowBalloonTip(5000, System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location), "Warframe has been detected", ToolTipIcon.Info);
            }
        }

        public void SetTextFailure()
        {
            if (systemTrayIcon != null)
            {
                systemTrayIcon.Text = "Warframe Market Overlay (Warframe is not running)";
                systemTrayIcon.ShowBalloonTip(5000, System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location), "Warframe has closed", ToolTipIcon.Info);
            }
        }

        private void DonateItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://google.com");  
        }   //placeholder

        private void HelpItem_Click(object sender, EventArgs e)
        {
            HelpForm help = new HelpForm();
            help.ShowDialog();
        }

        private void OptionsItem_Click(object sender, EventArgs e)
        {
            OptionsForm options = new OptionsForm();
            options.ShowDialog();
        }

        private void ExitItem_Click(object sender, EventArgs e)
        {
            relatedForm.Close();
        }

        public void Dispose()
        {
            if (systemTrayIcon != null)
            {
                systemTrayIcon.Visible = false;
                systemTrayIcon.Dispose();
            }
        }

    }
}
