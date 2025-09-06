using System;
namespace Veritas.Algorithms;

/// <summary>Provides the Luhn (mod 10) checksum algorithm.</summary>
internal static class Luhn
{
    /// <summary>Computes the Luhn check digit for the provided numeric string (without the check digit).</summary>
    public static int ComputeCheckDigit(ReadOnlySpan<char> digits)
    {
        int sum = 0;
        bool doubleDigit = true;
        for (int i = digits.Length - 1; i >= 0; i--)
        {
            int d = digits[i] - '0';
            if ((uint)d > 9) throw new ArgumentException("Non-digit character present", nameof(digits));
            if (doubleDigit)
            {
                d *= 2;
                if (d > 9) d -= 9;
            }
            sum += d;
            doubleDigit = !doubleDigit;
        }
        return (10 - (sum % 10)) % 10;
    }

    /// <summary>Validates that the supplied numeric string satisfies the Luhn checksum.</summary>
    public static bool Validate(ReadOnlySpan<char> digits)
    {
        if (digits.IsEmpty) return false;
        int sum = 0;
        bool doubleDigit = false;
        for (int i = digits.Length - 1; i >= 0; i--)
        {
            int d = digits[i] - '0';
            if ((uint)d > 9) return false;
            if (doubleDigit)
            {
                d *= 2;
                if (d > 9) d -= 9;
            }
            sum += d;
            doubleDigit = !doubleDigit;
        }
        return sum % 10 == 0;
    }
}
