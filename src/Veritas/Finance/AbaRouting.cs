using System;
using Veritas;

namespace Veritas.Finance;

/// <summary>Represents a validated US ABA routing number.</summary>
public readonly struct AbaRoutingValue
{
    public string Value { get; }
    public AbaRoutingValue(string value) => Value = value;
}

/// <summary>Provides validation for ABA routing numbers.</summary>
public static class AbaRouting
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<AbaRoutingValue> result)
    {
        Span<char> buffer = stackalloc char[9];
        if (!Normalize(input, buffer, out int len) || len != 9)
        {
            result = new ValidationResult<AbaRoutingValue>(false, default, ValidationError.Length);
            return true;
        }
        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            int d = buffer[i] - '0';
            if ((uint)d > 9)
            {
                result = new ValidationResult<AbaRoutingValue>(false, default, ValidationError.Charset);
                return true;
            }
            int weight = i % 3 == 0 ? 3 : i % 3 == 1 ? 7 : 1;
            sum += d * weight;
        }
        if (sum % 10 != 0)
        {
            result = new ValidationResult<AbaRoutingValue>(false, default, ValidationError.Checksum);
            return true;
        }
        string value = new string(buffer);
        result = new ValidationResult<AbaRoutingValue>(true, new AbaRoutingValue(value), ValidationError.None);
        return true;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int written)
    {
        written = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '\t' || ch == '-') continue;
            if (!char.IsDigit(ch)) { written = 0; return false; }
            if (written >= dest.Length) { written = 0; return false; }
            dest[written++] = ch;
        }
        return true;
    }
}

