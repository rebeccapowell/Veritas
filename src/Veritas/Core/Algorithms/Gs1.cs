using System;

namespace Veritas.Algorithms;

/// <summary>GS1 mod 10 algorithm with 3-1 weighting.</summary>
internal static class Gs1
{
    public static int ComputeCheckDigit(ReadOnlySpan<char> digits)
    {
        int sum = 0;
        bool three = true;
        for (int i = digits.Length - 1; i >= 0; i--)
        {
            int d = digits[i] - '0';
            if ((uint)d > 9) throw new ArgumentException("Non-digit character present", nameof(digits));
            sum += d * (three ? 3 : 1);
            three = !three;
        }
        return (10 - (sum % 10)) % 10;
    }

    public static bool Validate(ReadOnlySpan<char> digits)
    {
        if (digits.IsEmpty) return false;
        int sum = 0;
        bool three = false;
        for (int i = digits.Length - 1; i >= 0; i--)
        {
            int d = digits[i] - '0';
            if ((uint)d > 9) return false;
            sum += d * (three ? 3 : 1);
            three = !three;
        }
        return sum % 10 == 0;
    }
}
