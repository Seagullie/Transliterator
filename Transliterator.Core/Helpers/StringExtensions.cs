namespace Transliterator.Core.Helpers;

public static class StringExtensions
{
    public static bool HasUppercase(this string text)
    {
        foreach (char c in text)
        {
            if (char.IsUpper(c))
            {
                return true;
            }
        }
        return false;
    }
}