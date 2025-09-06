using System;
using System.Collections.Generic;

namespace Veritas;

/// <summary>Helpers for bulk identifier generation.</summary>
public static class Bulk
{
    /// <summary>Delegate matching generator signatures using <see cref="Span{T}"/>.</summary>
    public delegate (bool ok, int written) SpanGenerator(Span<char> destination);

    /// <summary>Generates a sequence of identifiers using the provided generator delegate.</summary>
    /// <param name="tryGenerate">Delegate that attempts to write an identifier into the provided span.</param>
    /// <param name="count">How many identifiers to produce.</param>
    /// <param name="seed">Optional seed for deterministic generation.</param>
    public static IEnumerable<string> GenerateMany(
        SpanGenerator tryGenerate,
        int count,
        int? seed = null)
    {
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        // Seed may be used by the generator via Random.Shared or similar.
        if (seed.HasValue)
        {
            _ = new Random(seed.Value); // initialize deterministic state for callers using Random.Shared
        }

        var buffer = new char[128];
        for (int i = 0; i < count; i++)
        {
            var span = buffer.AsSpan();
            var (ok, written) = tryGenerate(span);
            if (ok)
                yield return new string(span[..written]);
        }
    }
}
