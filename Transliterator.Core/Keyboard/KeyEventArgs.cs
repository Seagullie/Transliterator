﻿using Transliterator.Core.Enums;

namespace Transliterator.Core.Keyboard
{
    public class KeyEventArgs
    {
        public static KeyEventArgs KeyDown(VirtualKeyCode virtualKeyCode, string character = "")
        {
            return new KeyEventArgs
            {
                Key = virtualKeyCode,
                IsKeyDown = true,
                IsModifier = virtualKeyCode.IsModifier(),
                Character = character
            };
        }

        public static KeyEventArgs KeyUp(VirtualKeyCode virtualKeyCode, string character = "")
        {
            return new KeyEventArgs
            {
                Key = virtualKeyCode,
                IsKeyDown = false,
                IsModifier = virtualKeyCode.IsModifier(),
                Character = character
            };
        }

        /// <summary>
        ///     The key code itself
        /// </summary>
        public VirtualKeyCode Key { get; internal set; } = VirtualKeyCode.None;

        /// <summary>
        ///     Set this to true if the event is handled, other event-handlers in the chain will not be called
        /// </summary>
        public bool Handled { get; set; }

        public bool IsKeyDown { get; set; }

        public bool IsControl { get; set; }

        public bool IsAlt { get; set; }

        public bool IsShift { get; set; }

        public bool IsModifier { get; set; }

        public string Character { get; set;}

    }
}
