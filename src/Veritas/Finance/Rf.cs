using System;
using Veritas.Algorithms;
using Veritas;

namespace Veritas.Finance;

/// <summary>Represents an ISO 11649 RF creditor reference.</summary>
public readonly struct RfValue
{
    public string Value { get; }
    public RfValue(string value) => Value = value;
}

/// <summary>Provides validation for ISO 11649 RF references.</summary>
public static class Rf
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<RfValue> result)
    {
        Span<char> buffer = stackalloc char[25];
        if (!Normalize(input, buffer, out int len))
        {
            result = new ValidationResult<RfValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len < 5 || len > 25 || buffer[0] != 'R' || buffer[1] != 'F')
        {
            result = new ValidationResult<RfValue>(false, default, ValidationError.Format);
            return true;
        }
        Span<char> digits = stackalloc char[50];
        int idx = 0;
        for (int i = 4; i < len; i++) Append(digits, ref idx, buffer[i]);
        for (int i = 0; i < 4; i++) Append(digits, ref idx, buffer[i]);
        if (Iso7064.ComputeMod97(digits[..idx]) != 1)
        {
            result = new ValidationResult<RfValue>(false, default, ValidationError.Checksum);
            return true;
        }
        string value = new string(buffer[..len]);
        result = new ValidationResult<RfValue>(true, new RfValue(value), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 25)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        destination[0] = 'R';
        destination[1] = 'F';
        destination[2] = destination[3] = '0';
        for (int i = 4; i < 25; i++)
        {
            int v = rng.Next(36);
            destination[i] = v < 10 ? (char)('0' + v) : (char)('A' + v - 10);
        }
        Span<char> digits = stackalloc char[50];
        int idx = 0;
        for (int i = 4; i < 25; i++) Append(digits, ref idx, destination[i]);
        Append(digits, ref idx, 'R');
        Append(digits, ref idx, 'F');
        Append(digits, ref idx, '0');
        Append(digits, ref idx, '0');
        int check = 98 - Iso7064.ComputeMod97(digits[..idx]);
        destination[2] = (char)('0' + check / 10);
        destination[3] = (char)('0' + check % 10);
        written = 25;
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

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int written)
    {
        written = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '\t') continue;
            char u = char.ToUpperInvariant(ch);
            if (!(char.IsDigit(u) || (u >= 'A' && u <= 'Z'))) { written = 0; return false; }
            if (written >= dest.Length) { written = 0; return false; }
            dest[written++] = u;
        }
        return true;
    }
}

