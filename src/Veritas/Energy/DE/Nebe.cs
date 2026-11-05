using System;

namespace Veritas.Energy.DE;

/// <summary>A structurally and checksum-valid Netzbereichs-ID, without allocation verification.</summary>
/// <example>Nebe.TryValidate("F1137355920", out var result);</example>
public readonly struct NebeValue
{
    /// <summary>Gets the eleven-character canonical identifier.</summary>
    public string Value { get; }
    internal NebeValue(string value) => Value = value;
    /// <summary>Returns the canonical identifier.</summary>
    public override string ToString() => Value ?? string.Empty;
}

/// <summary>Validates NeBe-IDs under BDEW guide 1.2, sections 5 and 8.2.</summary>
/// <example>Nebe.TryGenerate(new GenerationOptions { Seed = 123 }, new char[11], out var written);</example>
public static class Nebe
{
    /// <summary>Validates the F prefix, positional alphabet and ASCII checksum; normalises case and spaces.</summary>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<NebeValue> result)
    {
        Span<char> buffer = stackalloc char[11];
        if (!BdewAlphanumericId.TryValidate(input, 'F', buffer, out ValidationError error))
        {
            result = new ValidationResult<NebeValue>(false, default, error);
            return false;
        }
        result = new ValidationResult<NebeValue>(true, new NebeValue(new string(buffer)), ValidationError.None);
        return true;
    }

    /// <summary>Generates synthetic test data which may coincide with an assigned NeBe-ID.</summary>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Generates a synthetic NeBe-ID, reproducibly when Seed is set; it does not allocate a network area.</summary>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
        => BdewAlphanumericId.TryGenerate('F', options, destination, out written);
}
