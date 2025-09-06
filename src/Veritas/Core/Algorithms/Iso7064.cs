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
}
