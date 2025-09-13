using System;
using Veritas.Algorithms;
using Veritas;

namespace Veritas.Finance;

/// <summary>Represents a validated SEPA Creditor Identifier.</summary>
/// <example>SepaCreditorIdentifier.TryValidate("DE74ZZZ09999999999", out var value);</example>
public readonly struct SepaCreditorIdentifierValue
{
    /// <summary>The normalized SEPA Creditor Identifier string.</summary>
    public string Value { get; }
    public SepaCreditorIdentifierValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for SEPA Creditor Identifiers.</summary>
public static class SepaCreditorIdentifier
{
    /// <summary>Validates the supplied SEPA Creditor Identifier.</summary>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<SepaCreditorIdentifierValue> result)
    {
        Span<char> buffer = stackalloc char[35];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            char c = ch;
            if (len < 2)
            {
                if (c >= 'a' && c <= 'z') c = (char)(c - 32);
                if (c < 'A' || c > 'Z') { result = new ValidationResult<SepaCreditorIdentifierValue>(false, default, ValidationError.Charset); return false; }
            }
            else if (len < 4)
            {
                if (c < '0' || c > '9') { result = new ValidationResult<SepaCreditorIdentifierValue>(false, default, ValidationError.Charset); return false; }
            }
            else
            {
                if (c >= 'a' && c <= 'z') c = (char)(c - 32);
                if (!((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z'))) { result = new ValidationResult<SepaCreditorIdentifierValue>(false, default, ValidationError.Charset); return false; }
            }
            if (len >= buffer.Length) { result = new ValidationResult<SepaCreditorIdentifierValue>(false, default, ValidationError.Length); return false; }
            buffer[len++] = c;
        }
        if (len < 8)
        {
            result = new ValidationResult<SepaCreditorIdentifierValue>(false, default, ValidationError.Length);
            return false;
        }
        Span<char> digits = stackalloc char[70];
        int idx = 0;
        for (int i = 4; i < len; i++) Append(digits, ref idx, buffer[i]);
        for (int i = 0; i < 4; i++) Append(digits, ref idx, buffer[i]);
        if (Iso7064.ComputeMod97(digits[..idx]) != 1)
        {
            result = new ValidationResult<SepaCreditorIdentifierValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<SepaCreditorIdentifierValue>(true, new SepaCreditorIdentifierValue(new string(buffer[..len])), ValidationError.None);
        return true;
    }

    /// <summary>Generates a valid SEPA Creditor Identifier into the provided destination span.</summary>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Generates a valid SEPA Creditor Identifier using the specified options.</summary>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        var cc = _countryCodes[rng.Next(_countryCodes.Length)];
        int nationalLen = 5 + rng.Next(15); // 5..19
        int total = 7 + nationalLen;
        if (destination.Length < total)
        {
            written = 0;
            return false;
        }
        destination[0] = cc[0];
        destination[1] = cc[1];
        destination[2] = destination[3] = '0';
        destination[4] = destination[5] = destination[6] = 'Z';
        for (int i = 0; i < nationalLen; i++)
            destination[7 + i] = (char)('0' + rng.Next(10));
        Span<char> digits = stackalloc char[70];
        int idx = 0;
        for (int i = 4; i < total; i++) Append(digits, ref idx, destination[i]);
        Append(digits, ref idx, destination[0]);
        Append(digits, ref idx, destination[1]);
        Append(digits, ref idx, '0');
        Append(digits, ref idx, '0');
        int check = 98 - Iso7064.ComputeMod97(digits[..idx]);
        destination[2] = (char)('0' + check / 10);
        destination[3] = (char)('0' + check % 10);
        written = total;
        return true;
    }

    private static void Append(Span<char> dest, ref int idx, char ch)
    {
        if (ch >= 'A' && ch <= 'Z')
        {
            int v = ch - 'A' + 10;
            dest[idx++] = (char)('0' + v / 10);
            dest[idx++] = (char)('0' + v % 10);
        }
        else
        {
            dest[idx++] = ch;
        }
    }

    private static readonly string[] _countryCodes = new[]
    {
        "AT","BE","BG","CY","CZ","DE","DK","EE","ES","FI","FR","GR","HR","HU","IE","IT","LT","LU","LV","MT","NL","PL","PT","RO","SE","SI","SK","GB"
    };
}
