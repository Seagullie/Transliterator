using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transliterator.Core.Keyboard;
using Transliterator.Core.Models;

namespace Transliterator.Core
{
    public class TransliteratorService
    {
        public TransliterationTable TransliterationTable { get; set; }

        public bool State { get; set; } = true;

        public TransliteratorService()
        {
        }
    }
}
