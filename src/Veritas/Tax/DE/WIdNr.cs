using System;

namespace Veritas.Tax.DE;

/// <summary>A structurally parsed W-IdNr. base; its check digit and allocation are not verified.</summary>
/// <example>WIdNr.TryValidateBaseStructure("DE123456788", out var result);</example>
public readonly struct WIdNrBaseValue
{
    /// <summary>Gets DE followed by nine ASCII digits, preserving zeros.</summary>
    public string Value { get; }
    internal WIdNrBaseValue(string value) => Value = value;
    /// <summary>Returns the canonical structural base.</summary>
    public override string ToString() => Value ?? string.Empty;
}

/// <summary>A structurally parsed W-IdNr. activity identifier; its checksum and allocation are not verified.</summary>
/// <example>WIdNr.TryValidateStructure("DE123456788-00001", out var result);</example>
public readonly struct WIdNrValue
{
    /// <summary>Gets the canonical base and five-digit distinguishing suffix.</summary>
    public string Value { get; }
    /// <summary>Gets the separately modelled structural base.</summary>
    public WIdNrBaseValue BaseNumber { get; }
    /// <summary>Gets the five-digit distinguishing number, preserving zeros.</summary>
    public string DistinguishingNumber { get; }
    internal WIdNrValue(string value, WIdNrBaseValue baseNumber, string distinguishingNumber)
    {
        Value = value; BaseNumber = baseNumber; DistinguishingNumber = distinguishingNumber;
    }
    /// <summary>Returns the canonical structural activity identifier.</summary>
    public override string ToString() => Value ?? string.Empty;
}

/// <summary>Provides explicitly structural W-IdNr. parsing and synthetic structure generation.</summary>
/// <remarks>WIdV section 1 and AO section 139c, checked 2026-11-04. No universal base-checksum algorithm is assumed from VAT format equivalence. Successful calls do not verify check digits or official assignment.</remarks>
/// <example>WIdNr.TryGenerateSyntheticStructure(new GenerationOptions { Seed = 42 }, 2, new char[17], out var written);</example>
public static class WIdNr
{
    /// <summary>Checks only DE plus nine ASCII digits; checksum and official assignment are not checked.</summary>
    public static bool TryValidateBaseStructure(ReadOnlySpan<char> input, out ValidationResult<WIdNrBaseValue> result)
    {
        result = new ValidationResult<WIdNrBaseValue>(false, default, ValidationError.Format);
        if (input.Length != 11 || input[0] is not ('D' or 'd') || input[1] is not ('E' or 'e')) return false;
        for (int i = 2; i < input.Length; i++) if (!char.IsAsciiDigit(input[i])) return false;
        Span<char> canonical = stackalloc char[11];
        input.CopyTo(canonical); canonical[0] = 'D'; canonical[1] = 'E';
        result = new ValidationResult<WIdNrBaseValue>(true, new WIdNrBaseValue(new string(canonical)), ValidationError.None);
        return true;
    }

    /// <summary>Checks full activity-identifier structure and suffix 00001–99999; the base checksum is not checked.</summary>
    public static bool TryValidateStructure(ReadOnlySpan<char> input, out ValidationResult<WIdNrValue> result)
    {
        result = new ValidationResult<WIdNrValue>(false, default, ValidationError.Format);
        if (input.Length != 17 || input[11] != '-') return false;
        int suffix = 0;
        for (int i = 12; i < 17; i++)
        {
            if (!char.IsAsciiDigit(input[i])) return false;
            suffix = suffix * 10 + input[i] - '0';
        }
        if (suffix == 0 || !TryValidateBaseStructure(input[..11], out var baseResult)) return false;
        string distinguishing = new string(input[12..]);
        string canonical = baseResult.Value.Value + "-" + distinguishing;
        result = new ValidationResult<WIdNrValue>(true, new WIdNrValue(canonical, baseResult.Value, distinguishing), ValidationError.None);
        return true;
    }

    /// <summary>Generates a synthetic structure with initial suffix 00001; no valid check digit or non-assignment is promised.</summary>
    public static bool TryGenerateSyntheticStructure(Span<char> destination, out int written)
        => TryGenerateSyntheticStructure(default, 1, destination, out written);

    /// <summary>Generates a seeded structural fixture with an explicit suffix, without calculating a base checksum.</summary>
    public static bool TryGenerateSyntheticStructure(in GenerationOptions options, int distinguishingNumber, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 17 || distinguishingNumber < 1 || distinguishingNumber > 99999) return false;
        TryGenerateSyntheticBaseStructure(options, destination[..11], out _);
        destination[11] = '-';
        for (int i = 16; i >= 12; i--)
        {
            destination[i] = (char)('0' + distinguishingNumber % 10);
            distinguishingNumber /= 10;
        }
        written = 17;
        return true;
    }

    /// <summary>Generates DE plus nine synthetic digits, reproducibly with Seed; no checksum or assignment is claimed.</summary>
    public static bool TryGenerateSyntheticBaseStructure(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 11) return false;
        var random = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        destination[0] = 'D'; destination[1] = 'E';
        for (int i = 2; i < 11; i++) destination[i] = (char)('0' + random.Next(10));
        written = 11;
        return true;
    }
}
