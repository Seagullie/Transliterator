namespace Transliterator.Core.Models;

public class TransliterationTable : SortedDictionary<string, string>
{
    // TODO: Store table name as field in JSON file
    public TransliterationTable(Dictionary<string, string> replacementMap, string name = "")
        : base(replacementMap, new StringLengthComparer())
    {
        Name = name;
        UpdateGraphemes();
        UpdateAlphabet();
    }

    public string Name { get; set; }

    // Alphabet is a set of all letters that can be transliterated.
    // Basically, all keys from the replacement map + any characters within those keys, if a key consist of more than one character
    public HashSet<char> Alphabet { get; private set; } = new();

    // Multigraph = more than one letter. Examples of MultiGraphs: ch, sh, zh,
    // while s, d, f are not MultiGraphs, but graphemes
    public HashSet<string> MultiGraphs { get; private set; } = new();

    // Grapheme = a single letter
    public HashSet<char> Graphemes { get; private set; } = new();

    // Isolated Grapheme = a single letter that doesn't appear in any MultiGraph
    public HashSet<char> IsolatedGraphemes { get; private set; } = new();

    // MultiGraphGraphemes = a single letter that appears in a MultiGraph
    public HashSet<char> MultiGraphGraphemes { get; private set; } = new();

    // punctuation, for example, does not have a case
    public HashSet<string> GraphemesWithoutCase { get; private set; } = new();

    private void UpdateAlphabet()
    {
        var newAlphabet = new HashSet<char>();

        foreach (string key in Keys)
        {
            if (key.Length > 1)
            {
                foreach (char subkey in key)
                {
                    newAlphabet.Add(subkey);
                }
            }
            else
            {
                newAlphabet.Add(key[0]);
            }
        }

        Alphabet = newAlphabet;
    }

    private void UpdateGraphemes()
    {
        MultiGraphs = Keys.Where(key => key.Length > 1).ToHashSet();

        Graphemes = Keys.Where(s => s.Length == 1)
                        .Select(s => s[0]).ToHashSet();

        IsolatedGraphemes = Keys.Where(s => s.Length == 1 && !IsPartOfMultiGraph(s))
                                .Select(s => s[0]).ToHashSet();

        MultiGraphGraphemes = Keys.Where(key => key.Length == 1 && !IsIsolatedGrapheme(key[0]))
                                  .Select(s => s[0]).ToHashSet();

        GraphemesWithoutCase = Keys.Where(key => key.ToUpper() == key.ToLower()).ToHashSet();
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

    public bool IsIsolatedGrapheme(char character)
    {
        character = char.ToLower(character);

        bool isInIsolatedGraphemeList = IsolatedGraphemes.Contains(character);
        return isInIsolatedGraphemeList;
    }

    public bool IsMultiGraphGrapheme(char character)
    {
        character = char.ToLower(character);

        bool isInGraphemeList = MultiGraphGraphemes.Contains(character);
        return isInGraphemeList;
    }

    public bool IsGraphemeWithoutCase(string text)
    {
        return GraphemesWithoutCase.Contains(text);
    }

    private class StringLengthComparer : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            if (x == null || y == null)
                return string.Compare(x, y);

            int result = y.Length.CompareTo(x.Length);
            if (result == 0)
                result = string.Compare(x, y);

            return result;
        }
    }
}