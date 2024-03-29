﻿// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;
using Transliterator.Core.Keyboard;
using Transliterator.Core.Native;

namespace Transliterator.Core.Enums;

/// <summary>
/// Extensions for VirtualKeyCode
/// </summary>
public static class VirtualKeyCodeExtensions
{
    /// <summary>
    /// Test if the VirtualKeyCode is a modifier key
    /// </summary>
    /// <param name="virtualKeyCode">VirtualKeyCode</param>
    /// <returns>bool</returns>
    public static bool IsModifier(this VirtualKeyCode virtualKeyCode)
    {
        bool isModifier = false;
        switch (virtualKeyCode)
        {
            case VirtualKeyCode.Capital:
            case VirtualKeyCode.NumLock:
            case VirtualKeyCode.Scroll:
            case VirtualKeyCode.LeftShift:
            case VirtualKeyCode.Shift:
            case VirtualKeyCode.RightShift:
            case VirtualKeyCode.Control:
            case VirtualKeyCode.LeftControl:
            case VirtualKeyCode.RightControl:
            case VirtualKeyCode.Menu:
            case VirtualKeyCode.LeftMenu:
            case VirtualKeyCode.RightMenu:
            case VirtualKeyCode.LeftWin:
            case VirtualKeyCode.RightWin:
                isModifier = true;
                break;
        }

        return isModifier;
    }

    public static char ToUnicode(this VirtualKeyCode virtualKeyCode)
    {
        byte[] keyboardState = new byte[255];

        short shiftKeyState = NativeMethods.GetAsyncKeyState(VirtualKeyCode.Shift);

        bool isShift = (shiftKeyState & 0x8000) != 0;

        if (isShift)
            keyboardState[(ushort)VirtualKeyCode.Shift] = 0x80;
        else
            keyboardState[(ushort)VirtualKeyCode.Shift] = 0;

        bool isCaps = NativeMethods.GetKeyState(VirtualKeyCode.Capital) > 0;

        uint scanCode = NativeMethods.MapVirtualKey((uint)virtualKeyCode, 0);

        IntPtr foregroundWindow = NativeMethods.GetForegroundWindow();
        uint threadId = NativeMethods.GetWindowThreadProcessId(foregroundWindow, IntPtr.Zero);
        IntPtr inputLocaleIdentifier = NativeMethods.GetKeyboardLayout(threadId);

        StringBuilder result = new StringBuilder(5);
        int charCount = NativeMethods.ToUnicodeEx((uint)virtualKeyCode, scanCode, keyboardState, result, result.Capacity, 0, inputLocaleIdentifier);
        if (charCount == -1)
        {
            return '\0';
        }

        var res = isCaps ? result.ToString().ToUpper() : result.ToString();

        return string.IsNullOrEmpty(res) ? '\0' : char.Parse(res);
    }
}