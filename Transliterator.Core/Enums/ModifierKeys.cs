namespace Transliterator.Core.Enums;

public enum ModifierKeys : uint
{
    None = 0, // No modifier keys
    Alt = 0x0001, // The Alt key
    Control = 0x0002, // The Control key
    Shift = 0x0004, // The Shift key
    Win = 0x0008, // The Windows key
    ModifiersMask = 0xFFFF, // A bitmask that isolates the modifier keys
    NoRepeat = 0x4000 // Indicates that the hotkey should not be generated repeatedly while the key is held down
}