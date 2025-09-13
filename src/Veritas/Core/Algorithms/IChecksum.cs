using System;

namespace Veritas.Algorithms;

/// <summary>Defines a checksum strategy with compute and verify operations.</summary>
public interface IChecksum
{
    /// <summary>Computes the check character(s) for the provided input.</summary>
    /// <param name="input">Input string without the check character(s).</param>
    /// <returns>Computed check character(s) as a string.</returns>
    string Compute(ReadOnlySpan<char> input);

    /// <summary>Verifies that the provided input matches the expected check character(s).</summary>
    /// <param name="input">Input string without the check character(s).</param>
    /// <param name="expected">Expected check character(s).</param>
    /// <returns><c>true</c> if the checksum matches; otherwise <c>false</c>.</returns>
    bool Verify(ReadOnlySpan<char> input, ReadOnlySpan<char> expected);

    /// <summary>Convenience method to compute a single check digit.</summary>
    /// <param name="input">Input string without the check digit.</param>
    /// <returns>The computed check digit.</returns>
    char ComputeCheckDigit(ReadOnlySpan<char> input) => Compute(input)[0];
}
