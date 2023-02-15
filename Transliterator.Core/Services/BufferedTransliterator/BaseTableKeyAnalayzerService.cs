using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transliterator.Services
{
    public class TableKeyAnalyzerService
    {
        // combo = more than one letter. Examples of combos: ch, sh, zh
        // while s d f are not combos
        private string[] combos;

        private LoggerService loggerService;

        // can't make it a singleton cause it depends on init parameter
        public TableKeyAnalyzerService(string[] combos)
        {
            loggerService = LoggerService.GetInstance();

            this.combos = combos;
        }

        public bool IsStartOfCombination(string text)
        {
            text = text.ToLower();

            foreach (string combo in combos)
            {
                if (combo.StartsWith(text) && combo.Length > text.Length) return true;
            };

            return false;
        }

        public bool IsPartOfCombination(string text)
        {
            text = text.ToLower();

            foreach (string combo in combos)
            {
                if (combo.Contains(text) && combo.Length > text.Length) return true;
            };

            return false;
        }

        public bool EndsWithComboInit(string text)
        {
            return IsStartOfCombination(text[text.Length - 1].ToString());
        }

        // aren't .is_part_of_combination and .EndsWithComboInit the same?
        // no, cause .EndsWithComboInit checks last character only, while .is_part_of_combination entire parameter on whether it is a substring of existing combo.

        public bool LastCharacterIsComboInit(string text)
        {
            foreach (string combo in combos)
            {
                if (text.EndsWith(combo)) return true;
            };

            return false;
        }

        public bool IsCombo(string text)
        {
            return combos.Contains(text);
        }

        // subcombo is a combo within another combo. Example: [sch] (щ) contains [ch] (ч). Thus, [ch] is a subcombo
        public bool IsSubComboFinisher(string character)
        {
            // TODO: Implement
            return false; // warning danger. Will implement later
        }

        public bool IsComboFinisher(string character)
        {
            foreach (string combo in combos)
            {
                if (combo.EndsWith(character))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsAddingUpToCombo(string prefix, string characterFinisher)
        {
            return IsCombo(prefix + characterFinisher);
        }

        // removes last character of text param and checks whether the remainder ends with combo init
        public bool EndsWithBrokenCombo(string text)
        {
            if (combos.Length == 0) return false;

            if (text.Length < 2) return false;

            if (IsCombo(text)) return false;

            string textWithoutLastCharacter = text.Substring(0, text.Length - 1);

            int longestComboLen = combos[0].Length; // combos are sorted and that's why it's enough to just get first element's length

            // clamping combo length to text length:
            int longestLocalComboLen = Math.Clamp(textWithoutLastCharacter.Length, 0, longestComboLen);

            for (int i = 1; i < longestLocalComboLen + 1; i++)
            {
                // TODO: Annotate
                // now check all the substrings on whether those are combo inits or not by slicing more and more of right side each time
                string substr = textWithoutLastCharacter.Substring(textWithoutLastCharacter.Length - i);
                if (EndsWithComboInit(substr))
                {
                    loggerService.LogMessage(this, $"broken combo detected: {text}");
                    return true;
                }
            }

            return false;
        }
    }
}