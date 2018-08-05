using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Management;
using Hotkeys;
using System.Collections.Generic;

namespace WarframeMarketOverlay
{
    public partial class GlobalHotkeyListener : Form
    {
        private Process warframeProcess;
        private WarframeQueryHandler queryHandler;
        private GlobalHotkey mainTrigger;
        private NotifyIcon systemTrayIcon;
        private List<ManagementEventWatcher> startupWatchers;

        ////////////////
        //Form methods//
        ////////////////

        public GlobalHotkeyListener()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Minimized;
            this.Hide();                //Makes it not show up on taskbar
            this.TopLevel = false;      //Makes it not show up under Apps in Task Manager
            startupWatchers = new List<ManagementEventWatcher>(2);
            this.InitializeSystemTray(); //Shows the system tray icon
        }

        private void GlobalHotkeyListener_Load(object sender, EventArgs e)
        {//Checks if warframe is running, if not, adds watchers

            try
            {
                Process temp = CheckForWarframe();
                if (temp != null)
                    WarframeIsRunning(temp);
                else
                {
                    var watcher1 = WatchForProcessStart("Notepad");
                    var watcher2 = WatchForProcessStart("Notepad.x64");
                    startupWatchers.Add(watcher1);
                    startupWatchers.Add(watcher2);
                    systemTrayIcon.ShowBalloonTip(10000, "Warframe Market Overlay", "Warframe is not runnnig!", ToolTipIcon.Info);
                }
            }
            catch (Exception a)
            {
                MessageBox.Show("Location: Hotkey Listener Load\r\nDetails: " + a.Message, "Error Message");
                this.Close();
            }
          
        }

        protected override void WndProc(ref Message m)
        {//Catches windows message that says our hotkey has been triggered

            if (m.Msg == Hotkeys.Constants.WM_HOTKEY_MSG_ID)
                HandleHotkey();
            base.WndProc(ref m);
        }   //Message loop that detects the hotkey

        private void GlobalHotkeyListener_FormClosing(object sender, FormClosingEventArgs e)
        {//Unregisters the hotkey and hides the tray icon

            //remove
            if (queryHandler != null)
                queryHandler.Dispose();

            systemTrayIcon.Visible = false;
            systemTrayIcon.Dispose();
            UnRegisterTriggerKey();
        }

        ////////////////////////////////////////////
        //Hotkey registration and handling methods//
        ////////////////////////////////////////////

        private void RegisterTriggerKey(Keys key, int mod = Constants.NOMOD)
        {//Creates and registers a trigger key according to passed values

            try
            {
                mainTrigger = new GlobalHotkey(mod, key, this);
                mainTrigger.Register();
            }
            catch (GlobalHotkeyException e)
            {
                MessageBox.Show("Location: Hotkey Listener Registration\r\nDetails: " + e.Message,"Error Message");
                this.BeginInvoke(new MethodInvoker(Close));
            }
        }

        private void UnRegisterTriggerKey()
        {//Unregisters the hotkey

            try
            {
                if (mainTrigger != null)
                    mainTrigger.Unregister();
            }
            catch (GlobalHotkeyException e)
            {
                MessageBox.Show("Location: Hotkey Listener Registration\r\nDetails: " + e.Message, "Error Message");
            }
        }

        private void HandleHotkey()
        {//Handles the hotkey (and does necessary checks)

            try
            {
                if (ForegroundWindow.IsInFocus(warframeProcess.MainWindowHandle) && queryHandler != null)
                    queryHandler.Execute();
            }
            catch (Exception e)
            {
                MessageBox.Show("Location: Hotkey Listener Handler Execution\r\nDetails: " + e.Message, "Error Message");
            }
        }

        ///////////////////////////////////////////////
        //Warframe detection and process registration//
        ///////////////////////////////////////////////

        private Process CheckForWarframe()
        {//throws exception if warframe isnt running

            Process[] temp = Process.GetProcessesByName("Notepad");
            if (temp.Length == 0)
            {
                temp = Process.GetProcessesByName("Notepad.x64");
                if (temp.Length == 0)
                {
                    //systemTrayIcon.ShowBalloonTip(4000, "Warframe Market Overlay", "Warframe is not running!", ToolTipIcon.Warning);
                    return null;
                }
            }
            return temp[0];
        }

        private ManagementEventWatcher WatchForProcessStart(string processName)
        {
            string queryString =
                "SELECT TargetInstance" +
                "  FROM __InstanceCreationEvent " +
                "WITHIN  1 " +
                " WHERE TargetInstance ISA 'Win32_Process' " +
                "   AND TargetInstance.Name = '" + processName + ".exe" + "'";

            // The dot in the scope means use the current machine
            string scope = @"\\.\root\CIMV2";

            // Create a watcher and listen for events
            ManagementEventWatcher watcher = new ManagementEventWatcher(scope, queryString);
            watcher.EventArrived += WarframeStartedEvent;
            watcher.Start();
            return watcher;
        }   //called if Warframe hasnt been detected

        private void WarframeStartedEvent(object sender, EventArrivedEventArgs e)   //called by the ManagementWatchers
        {//Registers the key and sets keyPressed to default value

            try
            {
                WarframeIsRunning(CheckForWarframe());
                foreach (var watcher in startupWatchers)
                {
                    if (watcher != null)
                    {
                        watcher.Stop();     //Has to be stopped because we dont know when GC will collect it
                        watcher.Dispose();
                    }
                }
            }
            catch (Exception a)
            {
                MessageBox.Show(this,"Location: Warframe Detection (Detected)\r\nDetails: " + a.Message, "Error Message");
            }
        }

        private void WarframeIsRunning(Process warframe)
        {
            if (warframe == null)
                throw new Exception("There has been an error with process registration.");

            SetTargetProcess(warframe);
            if (this.InvokeRequired)    //Registers the hotkey
            {
                this.Invoke(new MethodInvoker(delegate { RegisterTriggerKey(Properties.Settings.Default.Key_Value, Properties.Settings.Default.Modifier_Value); }));
            }
            else
            {
                RegisterTriggerKey(Properties.Settings.Default.Key_Value, Properties.Settings.Default.Modifier_Value);
            }
            systemTrayIcon.Text = "Warframe Market Overlay (Warframe is running)";
            systemTrayIcon.ShowBalloonTip(10000, "Warframe Market Overlay", "Warframe Detected", ToolTipIcon.Info);
        }   //called if Warframe has been detected

        private void SetTargetProcess(Process process)
        {//Links the passed process to this form

            this.warframeProcess = process;
            warframeProcess.Exited += new EventHandler(TargetProcess_Exited);
            warframeProcess.EnableRaisingEvents = true;
            queryHandler = new WarframeQueryHandler();
        }

        private void TargetProcess_Exited(object sender, EventArgs e)
        {//Exits the application when it's linked process closes
            
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate { MessageBox.Show(this, "Warframe and the application will close.", "Warframe Market Overlay"); }));
                this.Invoke(new MethodInvoker(delegate { this.Close(); }));
            }
            else
            {
                MessageBox.Show(this, "Warframe and the application will close.", "Warframe Market Overlay");
                this.Close();
            }
                
        }   //Called when Warframe is closing

        //////////////////////
        //System Tray events//
        //////////////////////

        private void InitializeSystemTray()
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
            systemTrayIcon.Text = "Warframe Market Overlay (Warframe is not running)";
            systemTrayIcon.Visible = true;
        }   //called when the form has loaded

        private void DonateItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://google.com");  //Change link
        }

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
            this.Close();
        }


    }
}
