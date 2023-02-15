using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transliterator.Core.Helpers.Events;
using Transliterator.Services;

namespace Transliterator.Core.Services.BufferedTransliterator
{
    public class Buffer : List<string>
    {
        // TODO: Refactor. Predicate requires returning bool and that's not what is necessary
        public event Predicate<string> ComboBrokenEvent;

        public Buffer()
        {
        }

        public void Add(string item, TableKeyAnalyzerService keyAnalyzer)
        {
            // sometimes combo can be broken by a character contributing towards bigger combo. e. g, "s" (combo init for "sh") can be broken by c and then followed by h for "sch" ("щ")

            if (keyAnalyzer.EndsWithBrokenCombo(GetAsString() + item) && !keyAnalyzer.IsPartOfCombination(GetAsString() + item))
            {
                {
                    ComboBrokenEvent?.Invoke(GetAsString());
                }
            }

            base.Add(item);
        }

        public string GetAsString()
        {
            return string.Join("", this);
        }
    }
}