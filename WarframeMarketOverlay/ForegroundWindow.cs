using System;
using System.Runtime.InteropServices;
[StructLayout(LayoutKind.Sequential)]
public struct RECT
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;
}

namespace WarframeMarketOverlay
{
    public static class ForegroundWindow
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        private static extern IntPtr GetShellWindow();
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowRect(IntPtr hwnd, out RECT rc);

        private static IntPtr desktopHandle; //Window handle for the desktop
        private static IntPtr shellHandle; //Window handle for the shell

        private static void GetHandles()
        {
            desktopHandle = GetDesktopWindow();
            shellHandle = GetShellWindow();
        }

        public static bool IsInFocus(IntPtr hWnd)
        {
            return GetForegroundWindow() == hWnd;
        }

        public static bool IsFullscreen(IntPtr hWnd)
        {   //Detect if the current app is running in full screen
            GetHandles();
            bool runningFullScreen = false;

            RECT appBounds;
            System.Drawing.Rectangle screenBounds;
            IntPtr current;

            //get the handle of the active window
            current = GetForegroundWindow();
            if (current.Equals(hWnd))
            {
                    //get the dimensions of the window of the app
                    GetWindowRect(current, out appBounds);

                    //get the screen dimensions on which the app is running
                    screenBounds = System.Windows.Forms.Screen.FromHandle(current).Bounds;

                    //determine if window is fullscreen
                    if ((appBounds.Bottom - appBounds.Top) == screenBounds.Height && (appBounds.Right - appBounds.Left) == screenBounds.Width)
                    {
                        runningFullScreen = true;
                    }
            }

            return runningFullScreen;
        }
    }
}
