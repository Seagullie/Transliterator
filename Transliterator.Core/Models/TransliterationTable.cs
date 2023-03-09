using System.Text;
using System.Text.RegularExpressions;
using Transliterator.Core.Keyboard;
using Transliterator.Helpers;

namespace Transliterator.Core.Models;

public partial class TransliterationTable : Dictionary<string, string>
{
    // TODO: Store table name as field in JSON file
    public TransliterationTable(Dictionary<string, string> replacementMap, string name = "") : base(replacementMap)
    {
        Name = name;
        UpdateGraphemes();
    }

    public string Name { get; set; }

    // Alphabet is a set of all letters that can be transliterated.
    // Basically, all keys from the replacement map + any characters within those keys, if a key consist of more than one character
    public string Alphabet { get; private set; } = string.Empty;

    // Multigraph = more than one letter. Examples of MultiGraphs: ch, sh, zh,
    // while s, d, f are not MultiGraphs, but graphemes
    public List<string> MultiGraphs { get; private set; } = new();

    // Grapheme = a single letter
    public List<string> Graphemes { get; private set; } = new();

    // Isolated Grapheme = a single letter that doesn't appear in any MultiGraph
    public List<string> IsolatedGraphemes { get; private set; } = new();

    // MultiGraphGraphemes = a single letter that appears in a MultiGraph
    public List<string> MultiGraphGraphemes { get; private set; } = new();

    public List<string> Keys { get; private set; } = new();

    public Dictionary<string, string> ReplacementMap { get; private set; } = new();

    private void UpdateAlphabet()
    {
        var newAlphabet = new StringBuilder();

        foreach (string key in Keys)
        {
            if (key.Length > 1)
            {
                foreach (char subkey in key)
                {
                    newAlphabet.Append(subkey);
                }
            }
            else
            {
                newAlphabet.Append(key);
            }
        }

        Alphabet = newAlphabet.ToString();
    }

    private void UpdateGraphemes()
    {
        MultiGraphs = Keys.Where(key => key.Length > 1).ToList();
        Graphemes = Keys.Where(key => key.Length == 1).ToList();

        IsolatedGraphemes = Keys.Where(key => key.Length == 1 && !IsPartOfMultiGraph(key)).ToList();
        MultiGraphGraphemes = Keys.Where(key => key.Length == 1 && !IsIsolatedGrapheme(key)).ToList();

        UpdateAlphabet();
    }

    public new void Add(string key, string value)
    {
        base.Add(key, value);
        UpdateGraphemes();
    }

    public new void Remove(string key)
    {
        base.Remove(key);
        UpdateGraphemes();
    }

    public override string? ToString()
    {
        if (string.IsNullOrEmpty(Name))
            return base.ToString();

        return Name;
    }

    // Removes last character of text param and checks whether the remainder ends with unfinished MultiGraph
    public bool IsBrokenMultiGraph(string text)
    {
        if (text.Length < 2 || IsMultiGraph(text) || MultiGraphs.Count == 0)
        {
            return false;
        }

        string textWithoutLastCharacter = text[..^1];
        return IsStartOfMultiGraph(textWithoutLastCharacter);
    }

    public bool IsAddingUpToMultiGraph(string prefix, string characterFinisher)
    {
        return IsMultiGraph(prefix + characterFinisher);
    }

    public bool IsMultiGraph(string text)
    {
        return MultiGraphs.Contains(text.ToLower());
    }

    public bool IsMultiGraphFinisher(string text)
    {
        text = text.ToLower();

        foreach (string mg in MultiGraphs)
        {
            if (mg.EndsWith(text))
            {
                return true;
            }
        }

        return false;
    }

    public bool IsPartOfMultiGraph(string text)
    {
        text = text.ToLower();

        foreach (string mg in MultiGraphs)
        {
            if (mg.Contains(text) && mg.Length > text.Length) return true;
        };

        return false;
    }

    public bool IsStartOfMultiGraph(string text)
    {
        text = text.ToLower();

        foreach (string mg in MultiGraphs)
        {
            if (mg.StartsWith(text) && mg.Length > text.Length)
                return true;
        };

        return false;
    }

    public bool IsIsolatedGrapheme(string text)
    {
        text = text.ToLower();

        bool isInIsolatedGraphemeList = IsolatedGraphemes.Contains(text);
        return isInIsolatedGraphemeList;
    }

    public bool IsMultiGraphGrapheme(string text)
    {
        text = text.ToLower();

        bool isInGraphemeList = MultiGraphGraphemes.Contains(text);
        return isInGraphemeList;
    }
}

public partial class TransliterationTable
{
    public string Transliterate(string text)
    {
        // Table keys and input text should have same case
        string inputText = text.ToLower();

        // loop over all possible user inputs and replace them with corresponding transliterations.
        // Replacement map is sorted, thus combinations will be transliterated first
        // This is because individual characters that make up a combination may also be present as separate replacements in the transliterationTableModel.replacementTable. By transliterating combinations first, the separate characters that make up the combination will not be replaced individually and will instead be treated as a single unit.
        // © ChatGPT
        foreach (string key in Keys)
        {
            // skip keys not present in the text
            if (inputText.Contains(key))
            {
                text = ReplaceKeepCase(key, this[key], text);
                // remove already transliterated keys from inputText. This is needed to prevent some bugs
                inputText = inputText.Replace(key, "");
            }
        }
        return text;
    }

    public string ReplaceKeepCase(string word, string replacement, string text)
    {
        Func<Match, string> onMatch = match =>
        {
            string matchString = match.Value;

            // nonalphabetic characters don't have uppercase
            // TODO: Optimize this part by making a dictionary for such characters when replacement map is installed
            if (!Utilities.HasUpperCase(matchString))
            {
                return GetCaseForNonalphabeticString(replacement);
            }

            if (Utilities.IsLowerCase(matchString)) return replacement.ToLower();
            if (char.IsUpper(matchString[0])) return replacement.ToUpper();
            // ^TODO: handle case when the replacement consists of several letters
            // if last character is uppercase, replacement should be uppercase as well:
            if (char.IsUpper(matchString[matchString.Length - 1])) return replacement.ToUpper();
            // not sure if C# has a method for converting to titlecase
            if (Utilities.IsUpperCase(matchString)) return replacement.ToUpper();

            return replacement;
        };

        return Regex.Replace(text, Regex.Escape(word), new MatchEvaluator(onMatch), RegexOptions.IgnoreCase);
    }

    // we could also copy case from previously entered character
    public string GetCaseForNonalphabeticString(string replacement)
    {
        if (KeyboardHook.IsCapsLockActive)
        {
            return replacement.ToUpper();
        }

        return replacement;
    }
}