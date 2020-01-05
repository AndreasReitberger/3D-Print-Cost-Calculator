﻿// Source: http://sanity-free.org/143/csharp_dotnet_single_instance_application.html

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace WpfFramework.Utilities
{    
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class SingleInstance
    {
        public const int HWND_BROADCAST = 0xffff;
        public static readonly int WM_SHOWME = RegisterWindowMessage("WM_SHOWME");
        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);
        [DllImport("user32")]
        public static extern int RegisterWindowMessage(string message);
    }
}
