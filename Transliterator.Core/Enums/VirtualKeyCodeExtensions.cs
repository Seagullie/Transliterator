﻿// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;
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

    public static string ToUnicode(this VirtualKeyCode virtualKeyCode)
    {
        byte[] keyboardState = new byte[255];
        bool keyboardStateStatus = NativeMethods.GetKeyboardState(keyboardState);

        if (!keyboardStateStatus)
        {
            return "";
        }

        uint scanCode = NativeMethods.MapVirtualKey((uint)virtualKeyCode, 0);
        IntPtr inputLocaleIdentifier = NativeMethods.GetKeyboardLayout(0);

        StringBuilder result = new StringBuilder();
        _ = NativeMethods.ToUnicodeEx((uint)virtualKeyCode, scanCode, keyboardState, result, 5, 0, inputLocaleIdentifier);

        return result.ToString();
    }
}