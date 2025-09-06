using System;

namespace Veritas.Algorithms;

internal static class Mod11
{
    public static int WeightedSum(ReadOnlySpan<char> digits, ReadOnlySpan<int> weights)
    {
        int sum = 0;
        for (int i = 0; i < weights.Length; i++)
            sum += (digits[i] - '0') * weights[i];
        return sum;
    }

    public static int ComputeMod11(ReadOnlySpan<char> digits, ReadOnlySpan<int> weights)
        => WeightedSum(digits, weights) % 11;
}

