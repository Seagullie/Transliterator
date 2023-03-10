using Transliterator.Core.Helpers;
using Transliterator.Core.Keyboard;
using Transliterator.Core.Models;

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

        public void RegisterHotKey(HotKey hotKey, Action action)
        {
            if (!_hotKeys.ContainsKey(hotKey))
            {
                _hotKeys.Add(hotKey, new List<Action>());
            }

            _hotKeys[hotKey].Add(action);
        }

        public void UnregisterHotKey(HotKey hotKey)
        {
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
    }
}
