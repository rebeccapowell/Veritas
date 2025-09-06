using System;

namespace Veritas.Algorithms;

/// <summary>Provides ISO 7064 checksum algorithms.</summary>
internal static class Iso7064
{
    /// <summary>Computes the Mod 97 remainder for the supplied numeric string.</summary>
    public static int ComputeMod97(ReadOnlySpan<char> digits)
    {
        int rem = 0;
        foreach (var ch in digits)
        {
            int d = ch - '0';
            if ((uint)d > 9) throw new ArgumentException("Non-digit character present", nameof(digits));
            rem = (rem * 10 + d) % 97;
        }
        return rem;
    }

    /// <summary>Computes the ISO 7064 Mod 97,10 check digits for the numeric string that contains a placeholder "00".</summary>
    public static int ComputeCheckDigitsMod97_10(ReadOnlySpan<char> digits)
    {
        int rem = ComputeMod97(digits);
        return 98 - rem;
    }

    /// <summary>Validates a numeric string with ISO 7064 Mod 97,10 checksum.</summary>
    public static bool ValidateMod97_10(ReadOnlySpan<char> digits)
    {
        if (digits.IsEmpty) return false;
        int rem = 0;
        foreach (var ch in digits)
        {
            int d = ch - '0';
            if ((uint)d > 9) return false;
            rem = (rem * 10 + d) % 97;
        }
        return rem == 1;
    }

    /// <summary>Computes the ISO 7064 Mod 11,10 check digit for the numeric string.</summary>
    public static char ComputeCheckDigitMod11_10(ReadOnlySpan<char> digits)
    {
        int check = 5;
        foreach (var ch in digits)
        {
            int d = ch - '0';
            if ((uint)d > 9) throw new ArgumentException("Non-digit character present", nameof(digits));
            check = (((check == 0 ? 10 : check) * 2) % 11 + d) % 10;
        }
        int temp = 1 - ((check == 0 ? 10 : check) * 2) % 11;
        int cd = temp % 10;
        if (cd < 0) cd += 10;
        return (char)('0' + cd);
    }

    /// <summary>Validates a numeric string with ISO 7064 Mod 11,10 checksum.</summary>
    public static bool ValidateMod11_10(ReadOnlySpan<char> digits)
    {
        if (digits.IsEmpty) return false;
        int check = 5;
        foreach (var ch in digits)
        {
            int d = ch - '0';
            if ((uint)d > 9) return false;
            check = (((check == 0 ? 10 : check) * 2) % 11 + d) % 10;
        }
        return check == 1;
    }

    /// <summary>Computes the ISO 7064 Mod 11,2 check digit for the numeric string.</summary>
    public static char ComputeCheckDigitMod11_2(ReadOnlySpan<char> digits)
    {
        int sum = 0;
        foreach (var ch in digits)
        {
            int d = ch - '0';
            if ((uint)d > 9) throw new ArgumentException("Non-digit character present", nameof(digits));
            sum = (sum + d) * 2 % 11;
        }
        int check = (12 - sum) % 11;
        return check == 10 ? 'X' : (char)('0' + check);
    }

    /// <summary>Validates a numeric string with ISO 7064 Mod 11,2 checksum.</summary>
    public static bool ValidateMod11_2(ReadOnlySpan<char> digits)
    {
        if (digits.Length < 2) return false;
        int sum = 0;
        for (int i = 0; i < digits.Length - 1; i++)
        {
            int d = digits[i] - '0';
            if ((uint)d > 9) return false;
            sum = (sum + d) * 2 % 11;
        }
        int check;
        char c = digits[^1];
        if (c == 'X' || c == 'x') check = 10;
        else { check = c - '0'; if ((uint)check > 9) return false; }
        return (sum + check) % 11 == 1;
    }

    private static int CharToMod37Value(char ch)
    {
        if (ch >= '0' && ch <= '9') return ch - '0';
        if (ch >= 'A' && ch <= 'Z') return ch - 'A' + 10;
        if (ch == '*') return 36;
        throw new ArgumentException("Invalid character", nameof(ch));
    }

    private static char ValueToMod37Char(int value)
        => value < 10 ? (char)('0' + value) : value < 36 ? (char)('A' + value - 10) : '*';

    /// <summary>Computes the ISO 7064 Mod 37,2 check character for the supplied string.</summary>
    public static char ComputeCheckCharacterMod37_2(ReadOnlySpan<char> input)
    {
        int p = 36;
        foreach (var ch in input)
        {
            int c = CharToMod37Value(ch);
            p = ((p + c) * 2) % 37;
        }
        int check = (38 - p) % 37;
        return ValueToMod37Char(check);
    }

    /// <summary>Validates a string with ISO 7064 Mod 37,2 checksum.</summary>
    public static bool ValidateMod37_2(ReadOnlySpan<char> input)
    {
        if (input.Length < 2) return false;
        char expected = ComputeCheckCharacterMod37_2(input[..^1]);
        return input[^1] == expected;
    }
}
