using System;
using Veritas.Algorithms;
using Veritas;

namespace Veritas.Finance;

/// <summary>Represents a validated SEPA Creditor Identifier.</summary>
/// <example>SepaCreditorIdentifier.TryValidate("DE98ZZZ09999999999", out var value);</example>
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
        if (!HasNationalStructure(buffer[..len]))
        {
            result = new ValidationResult<SepaCreditorIdentifierValue>(false, default, ValidationError.Format);
            return false;
        }
        int checkDigits = (buffer[2] - '0') * 10 + buffer[3] - '0';
        if (checkDigits < 2 || checkDigits > 98)
        {
            result = new ValidationResult<SepaCreditorIdentifierValue>(false, default, ValidationError.Checksum);
            return false;
        }
        Span<char> digits = stackalloc char[70];
        int idx = 0;
        for (int i = 7; i < len; i++) Append(digits, ref idx, buffer[i]);
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
        const string cc = "DE";
        const int nationalLen = 11;
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
        destination[7] = '0';
        Span<char> digits = stackalloc char[70];
        int idx = 0;
        for (int i = 7; i < total; i++) Append(digits, ref idx, destination[i]);
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

    // National structure: EPC262-08 v12.0, 2025-10-05. National checksums/assignment are not checked.
    private static bool HasNationalStructure(ReadOnlySpan<char> value)
    {
        ReadOnlySpan<char> national = value[7..];
        string? pattern = value[..2] switch
        {
            "AT" or "DE" or "HR" or "LV" or "SK" or "CH" or "LI" => "11n",
            "BE" => national.Length == 10 ? "10n" : "3n1a9n",
            "CY" => "4c",
            "CZ" or "GR" => "5n",
            "DK" or "NL" => "12n",
            "EE" => "2a11n",
            "FI" or "SI" => "8n",
            "FR" or "MC" => "6c",
            "IE" or "PT" => "6n",
            "IT" or "SM" => "16c",
            "LU" => "19c",
            "LT" => national.Length == 9 && national[0] == 'P' ? "1a8n" : "9n",
            "MT" => "9n1a",
            "NO" => "9n",
            "PL" or "SE" => "10n",
            "HU" => "1a8n",
            "ES" => "9c",
            "RO" => national.Length == 13 ? "13n" : national.Length is >= 2 and <= 10 ? national.Length + "n" : null,
            "BG" => national.Length is >= 9 and <= 25 ? national.Length + "c" : null,
            "GB" => national.Length is >= 13 and <= 28 ? "7a6n" + (national.Length == 13 ? "" : (national.Length - 13) + "c") : null,
            _ => null
        };
        if (pattern is null || !NationalFormat.Matches(national, pattern)) return false;
        return value[..2] switch
        {
            "AT" or "DE" or "LU" => national[0] == '0',
            "SK" => national[0] == '7',
            "CZ" => national[0] != '0',
            "BE" when national.Length == 13 => national[3] == 'D',
            "HU" => national[0] is 'A' or 'E',
            "BG" => national[0] == '2' ? national.Length >= 10 : national[0] == '1' && NationalFormat.Matches(national[1..], (national.Length - 1) + "n"),
            "MT" => "TMGLHC".IndexOf(national[^1]) >= 0,
            "GB" => national[..3].SequenceEqual("SDD"),
            _ => true
        };
    }
}
