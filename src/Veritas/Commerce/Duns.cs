using System;

namespace Veritas.Commerce;

/// <summary>A structurally normalised nine-digit D-U-N-S number; D&amp;B assignment is not verified.</summary>
/// <example>Duns.TryValidate("00-012-3456", out var result);</example>
public readonly struct DunsValue
{
    /// <summary>Gets the nine canonical ASCII digits, preserving leading zeros.</summary>
    public string Value { get; }
    internal DunsValue(string value) => Value = value;
    /// <summary>Returns the canonical number.</summary>
    public override string ToString() => Value ?? string.Empty;
}

/// <summary>Provides offline structural D-U-N-S validation and synthetic fixture generation.</summary>
/// <remarks>No Luhn or other checksum is assumed. Successful validation does not establish D&amp;B assignment, identity, registration or creditworthiness. No network lookup is performed.</remarks>
/// <example>Duns.TryGenerate(new GenerationOptions { Seed = 42 }, new char[9], out var written);</example>
public static class Duns
{
    /// <summary>Accepts exactly nine ASCII digits or the display form 12-345-6789; rejects other punctuation and whitespace.</summary>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<DunsValue> result)
    {
        result = new ValidationResult<DunsValue>(false, default, ValidationError.Format);
        bool display = input.Length == 11;
        if (input.Length != 9 && !display) return false;
        if (display && (input[2] != '-' || input[6] != '-')) return false;
        Span<char> digits = stackalloc char[9];
        int position = 0;
        for (int i = 0; i < input.Length; i++)
        {
            if (display && (i == 2 || i == 6)) continue;
            if (!char.IsAsciiDigit(input[i]))
            {
                result = new ValidationResult<DunsValue>(false, default, ValidationError.Charset);
                return false;
            }
            digits[position++] = input[i];
        }
        result = new ValidationResult<DunsValue>(true, new DunsValue(new string(digits)), ValidationError.None);
        return true;
    }

    /// <summary>Generates a synthetic nine-digit fixture; it may coincide with an assigned D-U-N-S number.</summary>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Generates reproducible synthetic digits when Seed is set; does not allocate an identity or promise production-safe enrichment inputs.</summary>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 9) return false;
        var random = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 9; i++) destination[i] = (char)('0' + random.Next(10));
        written = 9;
        return true;
    }
}
