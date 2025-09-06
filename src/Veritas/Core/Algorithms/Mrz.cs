using System;

namespace Veritas.Algorithms;

/// <summary>Machine Readable Zone check digit calculator.</summary>
internal static class Mrz
{
    private static readonly int[] Weights = {7,3,1};

    public static int Compute(ReadOnlySpan<char> input)
    {
        int sum = 0;
        for (int i = 0; i < input.Length; i++)
        {
            int v = Value(input[i]);
            if (v < 0) throw new ArgumentException("Invalid MRZ character", nameof(input));
            sum += v * Weights[i % 3];
        }
        return sum % 10;
    }

    public static bool Validate(ReadOnlySpan<char> input, char check)
        => Compute(input) == (check == '<' ? 0 : check - '0');

    private static int Value(char c)
    {
        if (c >= '0' && c <= '9') return c - '0';
        if (c == '<') return 0;
        c = char.ToUpperInvariant(c);
        if (c >= 'A' && c <= 'Z') return c - 'A' + 10;
        return -1;
    }
}
