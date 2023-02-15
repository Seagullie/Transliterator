using System.Collections.Generic;

namespace Transliterator.Core.Models;

public class TransliterationTable
{
    public string Name { get; set; }

    public Dictionary<string, string> ReplacementMap { get; private set; } = new();

    public List<string> Keys { get; private set; }

    // Combo = more than one letter. Examples of combos: ch, sh, zh,
    // while s, d, f are not combos
    public List<string> Combos { get; private set; } = new();

    // Alphabet is a set of all letters that can be transliterated.
    // Basically, all keys from the replacement map + any characters within those keys, if a key consist of more than one character
    public List<string> Alphabet { get; private set; } = new();

    // TODO: Store table name as field in JSON file
    public TransliterationTable(Dictionary<string, string> replacementMap, string name = "")
    {
        Name = name;
        ReplacementMap = replacementMap;
        UpdateKeys();
    }

    private void UpdateKeys()
    {
        Keys = ReplacementMap.Keys.OrderByDescending(key => key.Length).ToList();
        Combos = Keys.Where(key => key.Length > 1).ToList();
        UpdateAlphabet();
    }

    private void UpdateAlphabet()
    {
        Alphabet.Clear();

        foreach (string key in Keys)
        {
            if (key.Length > 1)
            {
                foreach (char subkey in key)
                {
                    Alphabet.Add(subkey.ToString());
                }
            }
            else
            {
                Alphabet.Add(key);
            }
        }
    }
}