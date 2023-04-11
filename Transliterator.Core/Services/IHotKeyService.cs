using Transliterator.Core.Models;

namespace Transliterator.Core.Services
{
    public interface IHotKeyService
    {
        bool IsHotkeyHandlingEnabled { get; set; }

        void RegisterHotKey(HotKey hotKey, Action action);
        void UnregisterHotKey(HotKey hotKey);
    }
}