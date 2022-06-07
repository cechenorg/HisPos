using System;
using System.Runtime.InteropServices;

namespace His_Pos.ChromeTabViewModel
{
    public class Win32
    {
        #region image preview api

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref W32Point pt);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, ref W32MonitorInfo lpmi);

        [DllImport("user32.dll")]
        internal static extern IntPtr MonitorFromPoint(W32Point pt, uint dwFlags);

        #endregion image preview api

        #region window api

        internal const uint GwHwndnext = 2;

        [DllImport("User32")]
        internal static extern IntPtr GetTopWindow(IntPtr hWnd);

        [DllImport("User32")]
        internal static extern IntPtr GetWindow(IntPtr hWnd, uint wCmd);

        #endregion window api
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct W32Point
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct W32MonitorInfo
    {
        public int Size;
        public W32Rect Monitor;
        public W32Rect WorkArea;
        public uint Flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct W32Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}