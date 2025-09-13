using System;

namespace Veritas.Algorithms;

internal static class Mod11
{
    /// <summary>Computes a weighted sum of the supplied numeric digits.</summary>
    /// <param name="digits">Numeric digits to weight.</param>
    /// <param name="weights">Weight pattern to apply. When shorter than <paramref name="digits"/> it repeats.</param>
    /// <param name="fromRight">Whether to apply weights starting from the right-most digit.</param>
    public static int WeightedSum(ReadOnlySpan<char> digits, ReadOnlySpan<int> weights, bool fromRight = false)
    {
        if (weights.IsEmpty) throw new ArgumentException("Weights cannot be empty", nameof(weights));

        int sum = 0;
        if (fromRight)
        {
            int wi = 0;
            for (int i = digits.Length - 1; i >= 0; i--)
            {
                int d = digits[i] - '0';
                if ((uint)d > 9) throw new ArgumentException("Non-digit character present", nameof(digits));
                sum += d * weights[wi];
                wi++;
                if (wi == weights.Length) wi = 0;
            }
        }
        else
        {
            int wi = 0;
            for (int i = 0; i < digits.Length; i++)
            {
                int d = digits[i] - '0';
                if ((uint)d > 9) throw new ArgumentException("Non-digit character present", nameof(digits));
                sum += d * weights[wi];
                wi++;
                if (wi == weights.Length) wi = 0;
            }
        }
        return sum;
    }

    /// <summary>Computes the mod 11 remainder for the weighted digits.</summary>
    public static int ComputeMod11(ReadOnlySpan<char> digits, ReadOnlySpan<int> weights, bool fromRight = false)
        => WeightedSum(digits, weights, fromRight) % 11;

    /// <summary>Computes the mod 11 check digit value for the supplied digits.</summary>
    public static int ComputeCheckDigit(ReadOnlySpan<char> digits, ReadOnlySpan<int> weights, bool fromRight = false)
    {
        int rem = ComputeMod11(digits, weights, fromRight);
        return (11 - rem) % 11;
    }

    /// <summary>Computes the mod 11 check character for the supplied digits, using 'X' to represent a value of 10.</summary>
    public static char ComputeCheckCharacter(ReadOnlySpan<char> digits, ReadOnlySpan<int> weights, bool fromRight = false)
    {
        int check = ComputeCheckDigit(digits, weights, fromRight);
        return check == 10 ? 'X' : (char)('0' + check);
    }

    /// <summary>Validates that the supplied string satisfies the weighted mod 11 checksum.</summary>
    public static bool Validate(ReadOnlySpan<char> input, ReadOnlySpan<int> weights, bool fromRight = false)
    {
        if (input.Length < 2) return false;
        int check;
        try
        {
            check = ComputeCheckDigit(input[..^1], weights, fromRight);
        }
        catch (ArgumentException)
        {
            return false;
        }
        char checkChar = input[^1];
        if (check == 10)
            return checkChar == 'X' || checkChar == 'x';
        return checkChar == (char)('0' + check);
    }
}

