using System.Text;
using System.Text.RegularExpressions;
using Transliterator.Core.Keyboard;
using Transliterator.Helpers;

namespace Transliterator.Core.Models;

// partial for properties
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
    }

    public new void Add(string key, string value)
    {
        base.Add(key, value);
        UpdateAlphabet();
        UpdateGraphemes();
    }

    public new void Remove(string key)
    {
        base.Remove(key);
        UpdateAlphabet();
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

// partial for transliteration methods
// not sure if transliteration should happen here or in it's own service
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

    public string ReplaceKeepCase(string replaceTarget, string replacement, string input)
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
    public string CarryOverCase(string sourceString, string finalString)
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
    public string GetCaseForNonalphabeticString(string replacement)
    {
        if (KeyboardHook.IsCapsLockActive)
        {
            return replacement.ToUpper();
        }

        return replacement;
    }
}