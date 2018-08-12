using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;

namespace WarframeMarketOverlay
{
    public delegate void ProcessHandler(Process p);

    class ProcessDetectorException : Exception
    {
        public ProcessDetectorException() { }
        public ProcessDetectorException(string message) : base(message) { }
        public ProcessDetectorException(string message, Exception inner) : base(message, inner) { }
    }

    class ProcessDetector : IDisposable
    {
        public string[] ProcessNames { get; }
        public ProcessHandler ReceivedDelegate { get; }
        private List<ManagementEventWatcher> startupWatchers;

        public ProcessDetector(string[] processNames, ProcessHandler processHandler)
        {
            if (processNames != null && processHandler != null)
            {
                ReceivedDelegate = processHandler;
                ProcessNames = new string[processNames.Length];
                for (int index = 0; index < ProcessNames.Length; index++)
                {
                    ProcessNames[index] = String.Copy(processNames[index]);
                }
            }
            else
            {
                throw new ProcessDetectorException("Constructor error", new NullReferenceException("Invalid argument."));
            }
        }

        public void Start()
        {
            try
            {
                var process = CheckForWarframe();
                if (process != null)
                    ReceivedDelegate(process);
                else
                    StartWatchers();
            }
            catch (Exception e)
            {
                Dispose();
                throw new ProcessDetectorException("Startup error", e);
            }
        }

        private Process CheckForWarframe()
        {
            Process[] temp = new Process[0];

            int max = ProcessNames.Length;
            int index = 0;

            while (temp.Length == 0 && index < max)
            {
                temp = Process.GetProcessesByName(ProcessNames[index]);
                index++;
            }

            if (temp.Length == 0)
                return null;
            else
                return temp[0];
        }

        private void StartWatchers()
        {
            startupWatchers = new List<ManagementEventWatcher>(ProcessNames.Length);
            foreach (var s in ProcessNames)
            {
                startupWatchers.Add(WatchForProcessStart(s));
            }
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
        }

        private void WarframeStartedEvent(object sender, EventArrivedEventArgs e)   //called by the ManagementWatchers
        {//Registers the key and sets keyPressed to default value

            ReceivedDelegate(CheckForWarframe());
            Dispose();
        }

        public void Dispose()
        {
                foreach (var watcher in startupWatchers)
                {
                    if (watcher != null)
                    {
                        watcher.Stop();     //Has to be stopped because we dont know when GC will collect it
                        watcher.Dispose();
                    }
                }
        }
    }
}
