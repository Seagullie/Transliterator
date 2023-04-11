using System.Text;
using Transliterator.Core.Enums;

namespace Transliterator.Core.Models;

public class HotKey
{
    public static HotKey None { get; } = new();
    public VirtualKeyCode Key { get; set; }
    public ModifierKeys Modifiers { get; set; }

    public HotKey(VirtualKeyCode keyCode, ModifierKeys modifiers)
    {
        Key = keyCode;
        Modifiers = modifiers;
    }

    public HotKey(uint keyCode, uint modifiers) : this((VirtualKeyCode)keyCode, (ModifierKeys)modifiers)
    {
    }

    public HotKey()
    {
        Key = VirtualKeyCode.None;
        Modifiers = ModifierKeys.None;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj is not HotKey)
        {
            return false;
        }
        HotKey other = (HotKey)obj;
        return Key == other.Key && Modifiers == other.Modifiers;
    }

    public override int GetHashCode()
    {
        return (int)Key ^ (int)Modifiers;
    }

    public override string ToString()
    {
        if (Key == VirtualKeyCode.None && Modifiers == ModifierKeys.None)
            return "< None >";

        var buffer = new StringBuilder();

        if (Modifiers.HasFlag(ModifierKeys.Control))
            buffer.Append("Ctrl + ");
        if (Modifiers.HasFlag(ModifierKeys.Shift))
            buffer.Append("Shift + ");
        if (Modifiers.HasFlag(ModifierKeys.Alt))
            buffer.Append("Alt + ");
        if (Modifiers.HasFlag(ModifierKeys.Win))
            buffer.Append("Win + ");

        buffer.Append(Key.GetDescriptionOrName());

        return buffer.ToString();
    }
}