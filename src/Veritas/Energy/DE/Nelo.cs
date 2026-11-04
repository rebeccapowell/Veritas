using System;

namespace Veritas.Energy.DE;

/// <summary>A structurally and checksum-valid Netzlokations-ID, without allocation verification.</summary>
/// <example>Nelo.TryValidate("E1137355921", out var result);</example>
public readonly struct NeloValue
{
    /// <summary>Gets the eleven-character canonical identifier.</summary>
    public string Value { get; }
    internal NeloValue(string value) => Value = value;
    /// <summary>Returns the canonical identifier.</summary>
    public override string ToString() => Value ?? string.Empty;
}

/// <summary>Validates NeLo-IDs under BDEW guide 1.2, sections 4 and 8.2.</summary>
/// <example>Nelo.TryGenerate(new GenerationOptions { Seed = 123 }, new char[11], out var written);</example>
public static class Nelo
{
    /// <summary>Validates the E prefix, positional alphabet and ASCII checksum; normalises case and spaces.</summary>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<NeloValue> result)
    {
        Span<char> buffer = stackalloc char[11];
        if (!BdewAlphanumericId.TryValidate(input, 'E', buffer, out ValidationError error))
        {
            result = new ValidationResult<NeloValue>(false, default, error);
            return false;
        }
        result = new ValidationResult<NeloValue>(true, new NeloValue(new string(buffer)), ValidationError.None);
        return true;
    }

    /// <summary>Generates synthetic test data which may coincide with an assigned NeLo-ID.</summary>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Generates a synthetic NeLo-ID, reproducibly when Seed is set; it does not allocate a network location.</summary>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
        => BdewAlphanumericId.TryGenerate('E', options, destination, out written);
}
