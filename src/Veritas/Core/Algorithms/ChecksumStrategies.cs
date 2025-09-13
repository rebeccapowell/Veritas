using System;
using LuhnAlgo = global::Veritas.Algorithms.Luhn;
using Mod11Algo = global::Veritas.Algorithms.Mod11;
using Iso7064Algo = global::Veritas.Algorithms.Iso7064;

namespace Veritas.Algorithms;

/// <summary>Provides reusable checksum strategies.</summary>
public static class ChecksumStrategies
{
    /// <summary>Gets an <see cref="IChecksum"/> for the numeric Luhn (mod 10) algorithm.</summary>
    public static IChecksum Luhn { get; } = new LuhnStrategy();

    /// <summary>Gets an <see cref="IChecksum"/> for the alphanumeric Luhn (base-36) algorithm.</summary>
    public static IChecksum LuhnBase36 { get; } = new LuhnBase36Strategy();

    /// <summary>Gets an <see cref="IChecksum"/> for GS1 mod 10 (weight pattern 3/1).</summary>
    public static IChecksum Gs1Mod10 { get; } = new Gs1Mod10Strategy();

    /// <summary>Creates a weighted mod 11 checksum strategy.</summary>
    public static IChecksum CreateWeightedMod11(ReadOnlySpan<int> weights, bool fromRight = false)
        => new WeightedMod11Strategy(weights.ToArray(), fromRight);

    /// <summary>Gets an <see cref="IChecksum"/> for ISO 7064 Mod 37,36.</summary>
    public static IChecksum Iso7064Mod37_36 { get; } = new Iso7064Mod37_36Strategy();

    private sealed class LuhnStrategy : IChecksum
    {
        public string Compute(ReadOnlySpan<char> input)
            => LuhnAlgo.ComputeCheckDigit(input).ToString();

        public bool Verify(ReadOnlySpan<char> input, ReadOnlySpan<char> expected)
            => expected.Length == 1 && LuhnAlgo.ComputeCheckDigit(input) == expected[0] - '0';
    }

    private sealed class LuhnBase36Strategy : IChecksum
    {
        public string Compute(ReadOnlySpan<char> input)
            => LuhnAlgo.ComputeCheckCharacterBase36(input).ToString();

        public bool Verify(ReadOnlySpan<char> input, ReadOnlySpan<char> expected)
            => expected.Length == 1 && char.ToUpperInvariant(LuhnAlgo.ComputeCheckCharacterBase36(input)) == char.ToUpperInvariant(expected[0]);
    }

    private sealed class Gs1Mod10Strategy : IChecksum
    {
        public string Compute(ReadOnlySpan<char> input)
        {
            int sum = 0;
            bool three = true;
            for (int i = input.Length - 1; i >= 0; i--)
            {
                int d = input[i] - '0';
                if ((uint)d > 9) throw new ArgumentException("Non-digit character present", nameof(input));
                sum += d * (three ? 3 : 1);
                three = !three;
            }
            int check = (10 - (sum % 10)) % 10;
            return ((char)('0' + check)).ToString();
        }

        public bool Verify(ReadOnlySpan<char> input, ReadOnlySpan<char> expected)
            => expected.Length == 1 && Compute(input)[0] == expected[0];
    }

    private sealed class WeightedMod11Strategy : IChecksum
    {
        private readonly int[] _weights;
        private readonly bool _fromRight;
        public WeightedMod11Strategy(int[] weights, bool fromRight)
        {
            _weights = weights;
            _fromRight = fromRight;
        }
        public string Compute(ReadOnlySpan<char> input)
            => Mod11Algo.ComputeCheckCharacter(input, _weights, _fromRight).ToString();

        public bool Verify(ReadOnlySpan<char> input, ReadOnlySpan<char> expected)
            => expected.Length == 1 && Mod11Algo.ComputeCheckCharacter(input, _weights, _fromRight) == expected[0];
    }

    private sealed class Iso7064Mod37_36Strategy : IChecksum
    {
        public string Compute(ReadOnlySpan<char> input)
            => Iso7064Algo.ComputeCheckCharacterMod37_36(input).ToString();

        public bool Verify(ReadOnlySpan<char> input, ReadOnlySpan<char> expected)
            => expected.Length == 1 && char.ToUpperInvariant(Iso7064Algo.ComputeCheckCharacterMod37_36(input)) == char.ToUpperInvariant(expected[0]);
    }
}
