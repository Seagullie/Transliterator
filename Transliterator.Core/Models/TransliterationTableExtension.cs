using System.Text;
using System.Text.RegularExpressions;
using Transliterator.Core.Helpers;
using Transliterator.Core.Keyboard;
using Transliterator.Helpers;

namespace Transliterator.Core.Models;

public static class TransliterationTableExtension
{
    public static string TestTransliterate(this TransliterationTable table, string input)
    {
        if (table.ContainsKey(input.ToLower()))
        {
            table.TryGetValue(input.ToLower(), out string? outputText);

            if (input.HasUppercase())
                outputText = outputText?.ToUpper();

            return outputText ?? string.Empty;
        }

        StringBuilder output = new StringBuilder(input.ToLower());

        foreach (string key in table.Keys)
        {
            int startIndex = 0;

            while (true)
            {
                // Find the next occurrence of the key in the input string
                int index = output.ToString().IndexOf(key, startIndex);

                if (index == -1)
                {
                    // Key not found, exit the loop
                    break;
                }

                // Get the character case of the input character
                bool isUpper = false;

                for (int i = 0; i < key.Length; i++)
                {
                    if (char.IsUpper(input[index + i]))
                    {
                        isUpper = true;
                        break;
                    }
                }

                // Replace the key with its transliterated value
                string replacement = table[key];

                if (isUpper)
                    replacement = replacement.ToUpper();

                output.Remove(index, key.Length);
                output.Insert(index, replacement);

                // Move the search index past the replacement string
                startIndex = index + replacement.Length;
            }
        }

        return output.ToString();
    }

    public static string Transliterate(this TransliterationTable table, string text)
    {
        // Table keys and input text should both be lowercase
        string inputText = text.ToLower();

        if (table.ContainsKey(inputText))
        {
            table.TryGetValue(inputText, out string? outputText);

            if (text.HasUppercase())
                outputText = outputText?.ToUpper();

            return outputText ?? string.Empty;
        }

        foreach (KeyValuePair<string, string> graphemes in table)
        {
            if (inputText.Contains(graphemes.Key))
            {
                text = ReplaceKeepCase(graphemes.Key, graphemes.Value, text);

                // Remove already transliterated keys from inputText.
                // This is needed to prevent some bugs
                inputText = inputText.Replace(graphemes.Key, "");
            }
        }

        return text ?? "";
    }

    private static string ReplaceKeepCase(string replaceTarget, string replacement, string input)
    {
        string onMatch(Match match)
        {
            string matchString = match.Value;
            string replacementWithTargetStringCasePreserved = CarryOverCase(matchString, replacement);
            return replacementWithTargetStringCasePreserved;
        }

        return Regex.Replace(input, Regex.Escape(replaceTarget), new MatchEvaluator((Func<Match, string>)onMatch), RegexOptions.IgnoreCase);
    }

    // carries over case from source string to final string
    // here's the mapping rules:
    // no case --> depends on capslock (for example, punctuation usually doesn't have case)
    // first character uppercase -> uppercase
    // last character uppercase -> uppercase
    // uppercase -> uppercase
    // lowercase -> lowercase
    private static string CarryOverCase(string sourceString, string finalString)
    {
        // nonalphabetic characters don't have uppercase
        // TODO: Optimize this part by making a dictionary for such characters when replacement map is installed
        if (!Utilities.HasUpperCase(sourceString))
        {
            return GetCaseForNonalphabeticString(finalString);
        }

        char firstSrcChar = sourceString[0];
        char lastSrcChar = sourceString[^1];

        if (Utilities.IsLowerCase(sourceString)) return finalString.ToLower();
        if (char.IsUpper(firstSrcChar)) return finalString.ToUpper();
        if (char.IsUpper(lastSrcChar)) return finalString.ToUpper();

        if (Utilities.IsUpperCase(sourceString)) return finalString.ToUpper();

        return finalString;
    }

    // we could also copy case from previously entered character
    private static string GetCaseForNonalphabeticString(string replacement)
    {
        if (KeyboardHook.GetKeyState(Enums.VirtualKeyCode.Capital))
        {
            return replacement.ToUpper();
        }

        return replacement;
    }
}