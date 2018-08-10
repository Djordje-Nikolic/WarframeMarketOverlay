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
        private SystemTrayIcon trayIcon;
        private ProcessDetector processDetector;

        public GlobalHotkeyListener()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Minimized;
            this.Hide();                //Makes it not show up on taskbar
            this.TopLevel = false;   //Makes it not show up under Apps in Task Manager


            trayIcon = null;            //Shows the system tray icon
            processDetector = null;
            queryHandler = null;
            warframeProcess = null;
            mainTrigger = null;
        }

        private void GlobalHotkeyListener_Load(object sender, EventArgs e)
        {//Checks if warframe is running, if not, adds watchers

            try
            {
                trayIcon = new SystemTrayIcon(this);
                trayIcon.Initialize();
                processDetector = new ProcessDetector(new string[] { "Notepad", "Notepad.x64" }, WarframeIsRunning);
                processDetector.Start();
            }
            catch (SystemTrayIconException a)
            {
                string temp = a.Message;
                if (a.InnerException != null)
                    temp += "\nInner Message: " + a.InnerException.Message;
                MessageBox.Show(this, temp, "System Tray Icon Error");
                this.Close();
            }
            catch (ProcessDetectorException a)
            {
                string temp = a.Message;
                if (a.InnerException != null)
                    temp += "\nInner Message: " + a.InnerException.Message;
                MessageBox.Show(this, temp, "Process Detector Error");
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

            if (queryHandler != null)
                queryHandler.Dispose();

            if (processDetector != null)
                processDetector.Dispose();

            UnRegisterTriggerKey();

            if (trayIcon != null)
                trayIcon.Dispose();
        }

        private void RegisterTriggerKey(Keys key, int mod = Constants.NOMOD)
        {
            try
            {
                mainTrigger = new GlobalHotkey(mod, key, this);
                mainTrigger.Register();
            }
            catch (GlobalHotkeyException e)
            {
                MessageBox.Show(e.Message,"Registration Error");
                this.BeginInvoke(new MethodInvoker(Close));
            }
        }

        private void UnRegisterTriggerKey()
        { 
            try
            {
                if (mainTrigger != null)
                    mainTrigger.Unregister();
            }
            catch (GlobalHotkeyException e)
            {
                MessageBox.Show(e.Message, "Registration Error");
            }
        }

        private void HandleHotkey()
        {//Handles the hotkey (and does necessary checks)

            try
            {
                if (ForegroundWindow.IsInFocus(warframeProcess.MainWindowHandle) && queryHandler != null)
                    queryHandler.Execute();
            }
            catch (QueryHandlerException e)
            {
                string temp = e.Message;
                if (e.InnerException != null)
                    temp += "\nInner Message: " + e.InnerException.Message;
                MessageBox.Show(temp, "Query Handler Error");
                this.Close();
            }
        }

        private void WarframeIsRunning(Process warframe)
        {
            try
            {
                warframeProcess = warframe;
                warframeProcess.Exited += new EventHandler(TargetProcess_Exited);
                warframeProcess.EnableRaisingEvents = true;

                queryHandler = new WarframeQueryHandler();
                processDetector = null;

                if (this.InvokeRequired)    //Registers the hotkey
                {
                    this.Invoke(new MethodInvoker(delegate { RegisterTriggerKey(Properties.Settings.Default.Key_Value, Properties.Settings.Default.Modifier_Value); }));
                }
                else
                {
                    RegisterTriggerKey(Properties.Settings.Default.Key_Value, Properties.Settings.Default.Modifier_Value);
                }
                trayIcon.SetTextSuccess();
            }
            catch (NullReferenceException e)
            {
                MessageBox.Show(e.Message, "Process Assignment Error");
                this.Close();
            }
        }   //called if Warframe has been detected

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
                
        }   //Called when Warframe is closing, rework to allow to stay open

    }
}
