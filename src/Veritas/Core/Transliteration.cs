using System;

namespace Veritas;

/// <summary>Provides transliteration helpers for various identifier schemes.</summary>
public static class Transliteration
{
    /// <summary>Attempts to transliterate a VIN character to its numeric value.</summary>
    public static bool TryVin(char ch, out int value)
    {
        if (ch >= '0' && ch <= '9') { value = ch - '0'; return true; }
        if (ch >= 'A' && ch <= 'Z')
        {
            value = ch switch
            {
                'A' => 1,
                'B' => 2,
                'C' => 3,
                'D' => 4,
                'E' => 5,
                'F' => 6,
                'G' => 7,
                'H' => 8,
                'J' => 1,
                'K' => 2,
                'L' => 3,
                'M' => 4,
                'N' => 5,
                'P' => 7,
                'R' => 9,
                'S' => 2,
                'T' => 3,
                'U' => 4,
                'V' => 5,
                'W' => 6,
                'X' => 7,
                'Y' => 8,
                'Z' => 9,
                _ => 0
            };
            return value != 0;
        }
        if (ch >= 'a' && ch <= 'z')
            return TryVin(char.ToUpperInvariant(ch), out value);
        value = 0;
        return false;
    }

    /// <summary>Attempts to transliterate an alphanumeric base-36 character.</summary>
    public static bool TryBase36(char ch, out int value)
    {
        if (ch >= '0' && ch <= '9') { value = ch - '0'; return true; }
        if (ch >= 'A' && ch <= 'Z') { value = ch - 'A' + 10; return true; }
        if (ch >= 'a' && ch <= 'z') { value = ch - 'a' + 10; return true; }
        value = 0;
        return false;
    }
}
