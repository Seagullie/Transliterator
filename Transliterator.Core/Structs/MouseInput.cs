// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

namespace Transliterator.Core.Structs;

/// <summary>
///     Contains information about a simulated mouse event.
///     See
///     <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms646273(v=vs.85).aspx">MOUSEINPUT structure</a>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct MouseInput
{
    /// <summary>
    ///     The absolute position of the mouse, or the amount of motion since the last mouse event was generated,
    ///     depending on the value of the dwFlags member.
    ///     Absolute data is specified as the x coordinate of the mouse;
    ///     relative data is specified as the number of pixels moved.
    /// </summary>
    private int dx;

    /// <summary>
    ///     The absolute position of the mouse, or the amount of motion since the last mouse event was generated,
    ///     depending on the value of the dwFlags member.
    ///     Absolute data is specified as the y coordinate of the mouse;
    ///     relative data is specified as the number of pixels moved.
    /// </summary>
    private int dy;

    /// <summary>
    ///     If dwFlags contains MOUSEEVENTF_WHEEL, then mouseData specifies the amount of wheel movement.
    ///     A positive value indicates that the wheel was rotated forward, away from the user;
    ///     a negative value indicates that the wheel was rotated backward, toward the user.
    ///     One wheel click is defined as WHEEL_DELTA, which is 120.
    ///     Windows Vista: If dwFlags contains MOUSEEVENTF_HWHEEL, then dwData specifies the amount of wheel movement.
    ///     A positive value indicates that the wheel was rotated to the right;
    ///     a negative value indicates that the wheel was rotated to the left.
    ///     One wheel click is defined as WHEEL_DELTA, which is 120.
    ///     If dwFlags does not contain MOUSEEVENTF_WHEEL, MOUSEEVENTF_XDOWN, or MOUSEEVENTF_XUP, then mouseData should be
    ///     zero.
    ///     If dwFlags contains MOUSEEVENTF_XDOWN or MOUSEEVENTF_XUP, then mouseData specifies which X buttons were pressed or
    ///     released.
    ///     This value may be any combination of the following flags:
    ///     XBUTTON1 0x0001 Set if the first X button is pressed or released.
    ///     XBUTTON2 0x0002 Set if the second X button is pressed or released.
    /// </summary>
    private int MouseData;

    /// <summary>
    ///     A set of bit flags that specify various aspects of mouse motion and button clicks.
    ///     The bits in this member can be any reasonable combination of the following values.
    /// </summary>
    private uint MouseEventFlags;

    /// <summary>
    ///     The time stamp for the event, in milliseconds. If this parameter is 0, the system will provide its own time stamp.
    /// </summary>
    private uint Timestamp;

    /// <summary>
    ///     An additional value associated with the mouse event. An application calls GetMessageExtraInfo to obtain this extra
    ///     information.
    /// </summary>
    private readonly UIntPtr dwExtraInfo;
}