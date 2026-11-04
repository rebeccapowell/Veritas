using System;

namespace Veritas.Finance.DE;

/// <summary>A Leitweg-ID with parsed routing components; assignment is not verified.</summary>
/// <example>LeitwegId.TryValidate("04011000-1234512345-06", out var result);</example>
public readonly struct LeitwegIdValue
{
    /// <summary>Gets the canonical routing identifier.</summary>
    public string Value { get; }
    /// <summary>Gets the coarse numeric address, preserving leading zeros.</summary>
    public string CoarseAddress { get; }
    /// <summary>Gets the optional fine address, or null when absent.</summary>
    public string? FineAddress { get; }
    /// <summary>Gets the two check digits, preserving a leading zero.</summary>
    public string CheckDigits { get; }
    internal LeitwegIdValue(string value, string coarse, string? fine, string checkDigits)
    {
        Value = value; CoarseAddress = coarse; FineAddress = fine; CheckDigits = checkDigits;
    }
    /// <summary>Returns the canonical routing identifier.</summary>
    public override string ToString() => Value ?? string.Empty;
}

/// <summary>Validates Leitweg-ID structure and MOD 97-10 checksums under KoSIT 2.0.2.</summary>
/// <remarks>Checks component syntax, not current AGS entries or routing registration. Peppol's 0204 scheme prefix is not part of a raw Leitweg-ID.</remarks>
/// <example>LeitwegId.TryGenerate("04011000", "1234512345", new char[46], out var written);</example>
public static class LeitwegId
{
    /// <summary>Checks mandatory separators and components, normalising ASCII letter case in the fine address.</summary>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<LeitwegIdValue> result)
    {
        result = new ValidationResult<LeitwegIdValue>(false, default, ValidationError.Format);
        if (input.Length < 5 || input.Length > 46) return false;
        int first = input.IndexOf('-'), last = input.LastIndexOf('-');
        if (first < 2 || last != input.Length - 3) return false;
        ReadOnlySpan<char> coarse = input[..first];
        ReadOnlySpan<char> fine = first == last ? ReadOnlySpan<char>.Empty : input[(first + 1)..last];
        if (first != last && fine.IsEmpty || !ValidCoarse(coarse) || !ValidFine(fine) ||
            !char.IsAsciiDigit(input[^2]) || !char.IsAsciiDigit(input[^1])) return false;
        int expected = ComputeCheckDigits(coarse, fine);
        if (expected != (input[^2] - '0') * 10 + input[^1] - '0')
        {
            result = new ValidationResult<LeitwegIdValue>(false, default, ValidationError.Checksum);
            return false;
        }
        Span<char> canonical = stackalloc char[46];
        input.CopyTo(canonical);
        for (int i = 0; i < fine.Length; i++) canonical[first + 1 + i] = char.ToUpperInvariant(fine[i]);
        string value = new string(canonical[..input.Length]);
        result = new ValidationResult<LeitwegIdValue>(true,
            new LeitwegIdValue(value, new string(coarse), fine.IsEmpty ? null : new string(canonical.Slice(first + 1, fine.Length)), value[^2..]), ValidationError.None);
        return true;
    }

    /// <summary>Builds a synthetic identifier from valid coarse and optional fine components, calculating check digits.</summary>
    /// <remarks>Does not allocate a routing identity. Input components may overlap the destination.</remarks>
    public static bool TryGenerate(ReadOnlySpan<char> coarseAddress, ReadOnlySpan<char> fineAddress, Span<char> destination, out int written)
    {
        written = 0;
        if (!ValidCoarse(coarseAddress) || !ValidFine(fineAddress)) return false;
        int required = coarseAddress.Length + (fineAddress.IsEmpty ? 0 : fineAddress.Length + 1) + 3;
        if (destination.Length < required) return false;
        int check = ComputeCheckDigits(coarseAddress, fineAddress);
        Span<char> buffer = stackalloc char[46];
        coarseAddress.CopyTo(buffer);
        int position = coarseAddress.Length;
        if (!fineAddress.IsEmpty)
        {
            buffer[position++] = '-';
            foreach (char c in fineAddress) buffer[position++] = char.ToUpperInvariant(c);
        }
        buffer[position++] = '-';
        buffer[position++] = (char)('0' + check / 10);
        buffer[position++] = (char)('0' + check % 10);
        buffer[..position].CopyTo(destination);
        written = position;
        return true;
    }

    /// <summary>Generates synthetic routing test data with a fine address; assignment is not established.</summary>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Generates reproducible synthetic routing data when a seed is supplied.</summary>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
        => TryGenerate(options, true, destination, out written);

    /// <summary>Generates synthetic data with or without a fine address; values may coincide with assigned identifiers.</summary>
    public static bool TryGenerate(in GenerationOptions options, bool includeFineAddress, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < (includeFineAddress ? 15 : 5)) return false;
        var random = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        int state = random.Next(1, 18);
        if (state == 17) state = 99;
        Span<char> coarse = stackalloc char[2];
        coarse[0] = (char)('0' + state / 10); coarse[1] = (char)('0' + state % 10);
        Span<char> fine = stackalloc char[9];
        const string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        if (includeFineAddress)
        {
            fine[0] = (char)('A' + random.Next(26));
            for (int i = 1; i < fine.Length; i++) fine[i] = alphabet[random.Next(alphabet.Length)];
        }
        return TryGenerate(coarse, includeFineAddress ? fine : ReadOnlySpan<char>.Empty, destination, out written);
    }

    private static bool ValidCoarse(ReadOnlySpan<char> value)
    {
        if (value.Length is not (2 or 3 or 5 or 8 or 9 or 12)) return false;
        foreach (char c in value) if (!char.IsAsciiDigit(c)) return false;
        int state = (value[0] - '0') * 10 + value[1] - '0';
        return state is >= 1 and <= 16 or 99;
    }

    private static bool ValidFine(ReadOnlySpan<char> value)
    {
        if (value.Length > 30) return false;
        foreach (char c in value) if (!char.IsAsciiLetterOrDigit(c)) return false;
        return true;
    }

    private static int ComputeCheckDigits(ReadOnlySpan<char> coarse, ReadOnlySpan<char> fine)
    {
        int remainder = 0;
        foreach (char c in coarse) remainder = (remainder * 10 + c - '0') % 97;
        foreach (char c in fine)
        {
            char upper = char.ToUpperInvariant(c);
            remainder = char.IsAsciiDigit(upper)
                ? (remainder * 10 + upper - '0') % 97
                : (remainder * 100 + upper - 'A' + 10) % 97;
        }
        return 98 - remainder * 100 % 97;
    }
}
