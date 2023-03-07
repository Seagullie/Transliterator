using Transliterator.Core.Enums;
using Transliterator.Core.Helpers;
using Transliterator.Core.Keyboard;

namespace Transliterator.Core.Services
{
    public class HotKeyService
    {
        private readonly KeyboardHook _keyboardHook;
        private readonly Dictionary<HotKey, List<Action>> _hotKeys = new();


        public HotKeyService()
        {
            _keyboardHook = Singleton<KeyboardHook>.Instance;
            _keyboardHook.KeyDown += HandleKeyPressed;
        }

        public void RegisterHotKey(uint keyCode, uint modifiers, Action action)
        {
            RegisterHotKey((VirtualKeyCode)keyCode, (ModifierKeys)modifiers, action);
        }

        public void RegisterHotKey(VirtualKeyCode keyCode, ModifierKeys modifiers, Action action)
        {
            var hotKey = new HotKey(keyCode, modifiers);
            if (!_hotKeys.ContainsKey(hotKey))
            {
                _hotKeys.Add(hotKey, new List<Action>());
            }

            _hotKeys[hotKey].Add(action);
        }

        public void UnregisterHotKey(uint keyCode, uint modifiers)
        {
            UnregisterHotKey((VirtualKeyCode)keyCode, (ModifierKeys)modifiers);
        }

        public void UnregisterHotKey(VirtualKeyCode keyCode, ModifierKeys modifiers)
        {
            var hotKey = new HotKey(keyCode, modifiers);
            if (_hotKeys.TryGetValue(hotKey, out var actions))
            {
                actions.Clear();
                _hotKeys.Remove(hotKey);
            }
        }

        private void HandleKeyPressed(object? sender, KeyboardHookEventArgs e)
        {
            var hotKey = new HotKey(e.Key, e.GetModifierKeys());

            var b = _hotKeys.ContainsKey(hotKey);
            if (_hotKeys.TryGetValue(hotKey, out var actions))
            {
                foreach (var action in actions)
                {
                    action();
                }
                e.Handled = true;
            }
        }

        private class HotKey
        {
            public VirtualKeyCode Key { get; set; }
            public ModifierKeys Modifiers { get; set; }

            public HotKey(VirtualKeyCode keyCode, ModifierKeys modifiers)
            {
                Key = keyCode;
                Modifiers = modifiers;
            }

            public override bool Equals(object? obj)
            {
                if (obj == null || !(obj is HotKey))
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
        }
    }
}
