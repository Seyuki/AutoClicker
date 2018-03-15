using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AutoClicker.Classes
{
    /// <summary>
    /// Static library class
    /// </summary>
    public static class ClickLib
    {
        #region Properties

        /// <summary>
        /// Left button pressed
        /// </summary>
        public static readonly int WM_LBUTTONDOWN = 0x0201;

        /// <summary>
        /// Left button released
        /// </summary>
        public static readonly int WM_LBUTTONUP = 0x0202;

        /// <summary>
        /// RECT struct
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Get the rect of the windows passed as a parameter
        /// </summary>
        /// <param name="hWnd">hWnd (window) to get RECT</param>
        /// <param name="lpRect">Window RECT</param>
        /// <returns>true if successful, false if failed</returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        /// <summary>
        /// Post a message to the hWnd
        /// </summary>
        /// <param name="hWnd">hWnd (window) that will receive the message</param>
        /// <param name="Msg">Message (input) to post</param>
        /// <param name="wParam">wParam</param>
        /// <param name="lParam">lParam (coordinates)</param>
        /// <returns>true if successful, false if failed</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);

        /// <summary>
        /// Get the foreground window
        /// </summary>
        /// <returns>Foreground window's hWnd</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// Get the window's process Id
        /// </summary>
        /// <param name="hWnd">Window</param>
        /// <param name="lpdwProcessId">Process ID</param>
        /// <returns> window's process Id</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        /// <summary>
        /// Get the hWnd attached to the process (by Name)
        /// </summary>
        /// <param name="ProcessName">Name of the process</param>
        /// <returns>hWnd attached to the process</returns>
        public static IntPtr GethWnd(string ProcessName)
        {
            Process[] Processes = Process.GetProcessesByName(ProcessName);
            return (Processes.Length > 0) ? Processes[0].MainWindowHandle : IntPtr.Zero;
        }

        /// <summary>
        /// Get the process to which hWnd is attached
        /// </summary>
        /// <param name="hWnd">hWnd to get process</param>
        /// <returns>Process name</returns>
        public static string GetProcessName(IntPtr hWnd)
        {
            try
            {
                uint actProcessId = 0;
                GetWindowThreadProcessId(hWnd, out actProcessId);
                Process actProcess = Process.GetProcessById((int)actProcessId);
                return actProcess.ProcessName;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Get the process' name in the foreground
        /// </summary>
        /// <returns>Process name</returns>
        public static string GetForegroundProcessName()
        {
            return GetProcessName(GetForegroundWindow());
        }

        /// <summary>
        /// Transform coordinates into IntPtr tu use PostMessage
        /// </summary>
        /// <param name="X">X Coordinate</param>
        /// <param name="Y">X Coordinate Y</param>
        /// <returns>Coordinates in IntPtr</returns>
        public static IntPtr lParam(uint? X, uint? Y) => (IntPtr)((Y << 16) | (X & 0xffff));

        #endregion Methods
    }
}
