// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Transliterator.Core.Enums;
using Transliterator.Core.Native;
using Transliterator.Core.Structs;

namespace Transliterator.Core.Keyboard;

/// <summary>
///     This is a utility class to help to generate input for mouse and keyboard
/// </summary>
public static class KeyboardInputGenerator
{
    /// <summary>
    ///     Generate key down
    /// </summary>
    /// <param name="keycodes">VirtualKeyCodes for the key downs</param>
    /// <returns>number of input events generated</returns>
    public static uint KeyDown(params VirtualKeyCode[] keycodes)
    {
        var keyboardInputs = new KeyboardInput[keycodes.Length];
        var index = 0;
        foreach (var virtualKeyCode in keycodes)
        {
            keyboardInputs[index++] = KeyboardInput.ForKeyDown(virtualKeyCode);
        }
        return NativeMethods.SendInput(Input.CreateKeyboardInputs(keyboardInputs));
    }

    /// <summary>
    ///     Generate a key combination press(es)
    /// </summary>
    /// <param name="keycodes">params VirtualKeyCodes</param>
    public static uint KeyCombinationPress(params VirtualKeyCode[] keycodes)
    {
        var keyboardInputs = new KeyboardInput[keycodes.Length * 2];
        var index = 0;
        // all down
        foreach (var virtualKeyCode in keycodes)
        {
            keyboardInputs[index++] = KeyboardInput.ForKeyDown(virtualKeyCode);
        }
        // all up
        foreach (var virtualKeyCode in keycodes)
        {
            keyboardInputs[index++] = KeyboardInput.ForKeyUp(virtualKeyCode);
        }

        return NativeMethods.SendInput(Input.CreateKeyboardInputs(keyboardInputs));
    }

    /// <summary>
    ///     Generate key press(es)
    /// </summary>
    /// <param name="keycodes">params VirtualKeyCodes</param>
    public static uint KeyPresses(params VirtualKeyCode[] keycodes)
    {
        var keyboardInputs = new KeyboardInput[keycodes.Length * 2];
        var index = 0;
        foreach (var virtualKeyCode in keycodes)
        {
            keyboardInputs[index++] = KeyboardInput.ForKeyDown(virtualKeyCode);
            keyboardInputs[index++] = KeyboardInput.ForKeyUp(virtualKeyCode);
        }

        return NativeMethods.SendInput(Input.CreateKeyboardInputs(keyboardInputs));
    }

    /// <summary>
    ///     Generate key(s) up
    /// </summary>
    /// <param name="keycodes">VirtualKeyCodes for the keys to release</param>
    /// <returns>number of input events generated</returns>
    public static uint KeyUp(params VirtualKeyCode[] keycodes)
    {
        var keyboardInputs = new KeyboardInput[keycodes.Length];
        var index = 0;
        foreach (var virtualKeyCode in keycodes)
        {
            keyboardInputs[index++] = KeyboardInput.ForKeyUp(virtualKeyCode);
        }
        return NativeMethods.SendInput(Input.CreateKeyboardInputs(keyboardInputs));
    }

    /// <summary>
    /// Calls the Win32 SendInput method with a stream of KeyDown and KeyUp messages in order to simulate uninterrupted text entry via the keyboard.
    /// </summary>
    /// <param name="text">The text to be simulated.</param>
    public static uint TextEntry(string text)
    {
        if (text.Length > uint.MaxValue / 2) 
            throw new ArgumentException(string.Format("The text parameter is too long. It must be less than {0} characters.", uint.MaxValue / 2), nameof(text));

        var keyboardInputs = new List<KeyboardInput>();

        foreach (char character in text)
        {
            keyboardInputs.AddRange(KeyboardInput.ForCharacter(character));
        }

        return NativeMethods.SendInput(Input.CreateKeyboardInputs(keyboardInputs.ToArray()));
    }

    /// <summary>
    /// Simulates a single character text entry via the keyboard.
    /// </summary>
    /// <param name="character">The unicode character to be simulated.</param>
    public static uint TextEntry(char character)
    {
        var keyboardInputs = KeyboardInput.ForCharacter(character);

        return NativeMethods.SendInput(Input.CreateKeyboardInputs(keyboardInputs));
    }
}