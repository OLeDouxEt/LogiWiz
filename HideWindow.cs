using System;
using System.Runtime.InteropServices;

namespace LogiWiz
{
    class HideWindow
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;

        public static void Hide()
        {
            // Hide the console window
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
        }
    }
}