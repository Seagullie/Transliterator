﻿using Transliterator.Core.Services;

namespace Transliterator.Core.Models;

public partial class TransliterationTable
{
    private LoggerService loggerService;

    // TODO: Store table name as field in JSON file
    public TransliterationTable(Dictionary<string, string> replacementMap, string name = "")
    {
        loggerService = LoggerService.GetInstance();

        Name = name;
        ReplacementMap = replacementMap;
        UpdateKeys();
    }

    // Alphabet is a set of all letters that can be transliterated.
    // Basically, all keys from the replacement map + any characters within those keys, if a key consist of more than one character
    public List<string> Alphabet { get; private set; } = new();

    // Combo = more than one letter. Examples of combos: ch, sh, zh,
    // while s, d, f are not combos
    public List<string> Combos { get; private set; } = new();

    public List<string> Keys { get; private set; } = new();
    public string Name { get; set; }

    public Dictionary<string, string> ReplacementMap { get; private set; } = new();

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

    private void UpdateKeys()
    {
        Keys = ReplacementMap.Keys.OrderByDescending(key => key.Length).ToList();
        Combos = Keys.Where(key => key.Length > 1).ToList();
        UpdateAlphabet();
    }

    public bool isInAlphabet(string key)
    {
        {
            return Alphabet.Contains(key.ToLower());
        }
    }

    // Removes last character of text param and checks whether the remainder ends with combo init
    public bool EndsWithBrokenCombo(string text)
    {
        if (text.Length < 2 || IsCombo(text) || Combos.Count == 0)
        {
            return false;
        }

        string textWithoutLastCharacter = text.Substring(0, text.Length - 1);

        // Clamping combo length to text length:
        int longestComboLen = Math.Clamp(Combos[0].Length, 0, textWithoutLastCharacter.Length); // textWithoutLastCharacter.Length < Combos[0].Length ? textWithoutLastCharacter.Length : Combos[0].Length;

        for (int i = 1; i < longestComboLen + 1; i++)
        {
            // Now check all the substrings on whether those are combo inits or not by slicing more and more each time
            string substr = textWithoutLastCharacter.Substring(textWithoutLastCharacter.Length - i);
            if (EndsWithComboInit(substr))
            {
                loggerService.LogMessage(this, $"broken combo detected: {text}");
                return true;
            }
        }

        return false;
    }

    public bool EndsWithComboInit(string text)
    {
        return IsStartOfCombination(text[text.Length - 1].ToString());
    }

    public bool IsAddingUpToCombo(string prefix, string characterFinisher)
    {
        return IsCombo(prefix + characterFinisher);
    }

    public bool IsCombo(string text)
    {
        return Combos.Contains(text.ToLower());
    }

    public bool IsComboFinisher(string character)
    {
        character = character.ToLower();

        foreach (string combo in Combos)
        {
            if (combo.EndsWith(character))
            {
                return true;
            }
        }

        return false;
    }

    public bool IsPartOfCombination(string text)
    {
        text = text.ToLower();

        foreach (string combo in Combos)
        {
            if (combo.Contains(text) && combo.Length > text.Length) return true;
        };

        return false;
    }

    public bool IsStartOfCombination(string text)
    {
        text = text.ToLower();

        foreach (string combo in Combos)
        {
            if (combo.StartsWith(text) && combo.Length > text.Length) return true;
        };

        return false;
    }
}