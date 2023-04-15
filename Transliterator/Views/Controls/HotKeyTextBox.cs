using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Transliterator.Core.Models;

// References:
// https://github.com/Tyrrrz/LightBulb

namespace Transliterator.Views.Controls;

public class HotKeyTextBox : TextBox
{
    public static readonly DependencyProperty HotKeyProperty = DependencyProperty.Register(
        nameof(HotKey),
        typeof(HotKey),
        typeof(HotKeyTextBox),
        new FrameworkPropertyMetadata(
            default(HotKey),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            HotKeyChanged
        )
    );

    public HotKeyTextBox()
    {
        IsReadOnly = true;
        IsReadOnlyCaretVisible = false;
        IsUndoEnabled = false;

        if (ContextMenu is not null)
            ContextMenu.Visibility = Visibility.Collapsed;

        Text = HotKey.ToString();
    }

    public HotKey HotKey
    {
        get => (HotKey)GetValue(HotKeyProperty);
        set => SetValue(HotKeyProperty, value);
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        e.Handled = true;

        // Get modifiers and key data
        var modifiers = Keyboard.Modifiers;
        var key = e.Key;

        // If nothing was pressed - return
        if (key == Key.None)
            return;

        // If Alt is used as modifier - the key needs to be extracted from SystemKey
        if (key == Key.System)
            key = e.SystemKey;

        // If Delete/Backspace/Escape is pressed without modifiers - clear current value and return
        if (key is Key.Delete or Key.Back or Key.Escape && modifiers == ModifierKeys.None)
        {
            HotKey = HotKey.None;
            return;
        }

        // If the only key pressed is one of the modifier keys - return
        if (key is
            Key.LeftCtrl or Key.RightCtrl or Key.LeftAlt or Key.RightAlt or
            Key.LeftShift or Key.RightShift or Key.LWin or Key.RWin or
            Key.Clear or Key.OemClear or Key.Apps)
            return;

        // If Enter/Space/Tab is pressed without modifiers - return
        if (key is Key.Enter or Key.Space or Key.Tab && modifiers == ModifierKeys.None)
            return;

        // If key has a character and pressed without modifiers or only with Shift - return
        if (HasKeyChar(key) && modifiers is ModifierKeys.None or ModifierKeys.Shift)
            return;

        // If the system key pressed - return
        if (key is
            Key.Escape or Key.Tab or Key.Insert or Key.NumLock or
            Key.CapsLock or Key.Delete or Key.Back or Key.Return or Key.Oem3)
            return;

        // Set value
        HotKey = new HotKey((uint)KeyInterop.VirtualKeyFromKey(key), (uint)modifiers);
    }

    private static bool HasKeyChar(Key key) => key is
        // A - Z
        >= Key.A and <= Key.Z or
        // 0 - 9
        >= Key.D0 and <= Key.D9 or
        // Numpad 0 - 9
        >= Key.NumPad0 and <= Key.NumPad9 or
        // The rest
        Key.OemQuestion or Key.OemQuotes or Key.OemPlus or Key.OemOpenBrackets or Key.OemCloseBrackets or
        Key.OemMinus or Key.DeadCharProcessed or Key.Oem1 or Key.Oem5 or Key.Oem7 or Key.OemPeriod or
        Key.OemComma or Key.Add or Key.Divide or Key.Multiply or Key.Subtract or Key.Oem102 or Key.Decimal;

    private static void HotKeyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is HotKeyTextBox control)
        {
            control.Text = control.HotKey.ToString();
        }
    }
}