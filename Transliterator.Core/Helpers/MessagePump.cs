﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Transliterator.Core.Native;

namespace Transliterator.Helpers
{
    public class MessagePump
    {
        [Serializable]
        public struct MSG
        {
            public IntPtr hwnd;

            public IntPtr lParam;

            public int message;

            public int pt_x;

            public int pt_y;

            public int time;

            public IntPtr wParam;
        }

        [DllImport("user32.dll")]
        public static extern bool GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [DllImport("user32.dll")]
        public static extern bool TranslateMessage([In] ref MSG lpMsg);

        [DllImport("user32.dll")]
        public static extern IntPtr DispatchMessage([In] ref MSG lpmsg);

        public static void Pump()
        {
            MSG msg;

            while ((!GetMessage(out msg, IntPtr.Zero, 0, 0)))
            {
                TranslateMessage(ref msg);
                DispatchMessage(ref msg);
            }
        }

        public static void PumpOnce()
        {
            MSG msg;

            GetMessage(out msg, IntPtr.Zero, 0, 0);
        }
    }
}