using System;

namespace Veritas.Energy.DE;

/// <summary>The issuing scheme of a BDEW/DVGW market partner identifier.</summary>
public enum MarketPartnerScheme
{
    /// <summary>BDEW electricity scheme, prefix 99 and allocation mode 0–8.</summary>
    BdewElectricity,
    /// <summary>DVGW gas scheme, prefix 98 and allocation mode 0–9.</summary>
    DvgwGas
}

/// <summary>A structurally valid BDEW/DVGW MP-ID; allocation is not verified.</summary>
/// <example>MarketPartnerId.TryValidate("9900000000003", out var result);</example>
public readonly struct MarketPartnerIdValue
{
    /// <summary>Gets the thirteen canonical ASCII digits.</summary>
    public string Value { get; }
    /// <summary>Gets the issuing scheme and sector.</summary>
    public MarketPartnerScheme Scheme { get; }
    /// <summary>Gets the allocation-mode digit at position three.</summary>
    public int AllocationMode { get; }
    internal MarketPartnerIdValue(string value, MarketPartnerScheme scheme)
    {
        Value = value;
        Scheme = scheme;
        AllocationMode = value[2] - '0';
    }
    /// <summary>Returns the canonical identifier.</summary>
    public override string ToString() => Value ?? string.Empty;
}

/// <summary>Validates BDEW/DVGW MP-IDs and generates synthetic, potentially assigned examples.</summary>
/// <remarks>Implements BDEW guide 1.2, sections 2 and 8.1. GS1 GLNs use a separate scheme.</remarks>
/// <example>MarketPartnerId.TryGenerate(MarketPartnerScheme.DvgwGas, new char[13], out var written);</example>
public static class MarketPartnerId
{
    /// <summary>Checks structure and checksum, normalising spaces, with at most 4,096 raw characters.</summary>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<MarketPartnerIdValue> result)
    {
        result = new ValidationResult<MarketPartnerIdValue>(false, default, ValidationError.Format);
        if (input.Length > 4096) return false;
        Span<char> digits = stackalloc char[13];
        int count = 0;
        foreach (char c in input)
        {
            if (c == ' ') continue;
            if (!char.IsAsciiDigit(c) || count == digits.Length) return false;
            digits[count++] = c;
        }
        if (count != 13 || digits[0] != '9' || digits[1] is not ('8' or '9')) return false;
        MarketPartnerScheme scheme = digits[1] == '9' ? MarketPartnerScheme.BdewElectricity : MarketPartnerScheme.DvgwGas;
        if (scheme == MarketPartnerScheme.BdewElectricity && digits[2] == '9') return false;
        if (BdewChecksum.ComputeNumeric(digits[..12]) != digits[12] - '0')
        {
            result = new ValidationResult<MarketPartnerIdValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<MarketPartnerIdValue>(true, new MarketPartnerIdValue(new string(digits), scheme), ValidationError.None);
        return true;
    }

    /// <summary>Checks an identifier against an explicitly required issuing scheme.</summary>
    public static bool TryValidate(ReadOnlySpan<char> input, MarketPartnerScheme expectedScheme, out ValidationResult<MarketPartnerIdValue> result)
    {
        if (!TryValidate(input, out result)) return false;
        if (result.Value.Scheme == expectedScheme) return true;
        result = new ValidationResult<MarketPartnerIdValue>(false, default, ValidationError.Format);
        return false;
    }

    /// <summary>Generates a synthetic example for the selected scheme; this does not allocate an MP-ID.</summary>
    public static bool TryGenerate(MarketPartnerScheme scheme, Span<char> destination, out int written)
        => TryGenerate(scheme, default, destination, out written);

    /// <summary>Generates a reproducible synthetic example when a seed is supplied.</summary>
    public static bool TryGenerate(MarketPartnerScheme scheme, in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 13 || scheme is not (MarketPartnerScheme.BdewElectricity or MarketPartnerScheme.DvgwGas)) return false;
        var random = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        destination[0] = '9';
        destination[1] = scheme == MarketPartnerScheme.BdewElectricity ? '9' : '8';
        destination[2] = (char)('0' + random.Next(scheme == MarketPartnerScheme.BdewElectricity ? 9 : 10));
        for (int i = 3; i < 12; i++) destination[i] = (char)('0' + random.Next(10));
        destination[12] = (char)('0' + BdewChecksum.ComputeNumeric(destination[..12]));
        written = 13;
        return true;
    }
}
