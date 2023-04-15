using Transliterator.Core.Models;

namespace Transliterator.Core.Services;

public interface IHotKeyService
{
    bool IsHotkeyHandlingEnabled { get; set; }

    bool RegisterHotKey(HotKey hotKey, Action action);

    bool UnregisterHotKey(HotKey hotKey);
}