using System.Text;

namespace Transliterator.Core.Models;

public class TransliterationTable : Dictionary<string, string>
{
    // TODO: Store table name as field in JSON file
    public TransliterationTable(Dictionary<string, string> replacementMap, string name = "") : base(replacementMap)
    {
        Name = name;
        UpdateGraphemes();
        UpdateAlphabet();
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