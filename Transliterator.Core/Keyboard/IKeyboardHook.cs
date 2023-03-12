using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transliterator.Core.Keyboard
{
    public interface IKeyboardHook
    {
        event EventHandler<KeyboardHookEventArgs>? KeyDown;
    }
}
