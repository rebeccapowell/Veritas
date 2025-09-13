using System;

namespace Veritas.Healthcare;

/// <summary>Represents a validated U.S. DEA registration number.</summary>
/// <example>DeaNumber.TryValidate("AB1234563", out var result);</example>
public readonly struct DeaNumberValue
{
    /// <summary>The normalized DEA number string.</summary>
    public string Value { get; }
    public DeaNumberValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for U.S. DEA registration numbers.</summary>
public static class DeaNumber
{
    /// <summary>Validates the supplied DEA number.</summary>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<DeaNumberValue> result)
    {
        Span<char> buffer = stackalloc char[9];
        int len = 0;
        foreach (var chRaw in input)
        {
            char ch = chRaw;
            if (ch == ' ' || ch == '-') continue;
            if (len < 2)
            {
                if (ch >= 'a' && ch <= 'z') ch = (char)(ch - 32);
                if ((ch < 'A' || ch > 'Z') && !(len == 1 && ch == '9'))
                {
                    result = new ValidationResult<DeaNumberValue>(false, default, ValidationError.Charset);
                    return false;
                }
            }
            else
            {
                if (ch < '0' || ch > '9')
                {
                    result = new ValidationResult<DeaNumberValue>(false, default, ValidationError.Charset);
                    return false;
                }
            }
            if (len >= 9)
            {
                result = new ValidationResult<DeaNumberValue>(false, default, ValidationError.Length);
                return false;
            }
            buffer[len++] = ch;
        }
        if (len != 9)
        {
            result = new ValidationResult<DeaNumberValue>(false, default, ValidationError.Length);
            return false;
        }
        int d1 = buffer[2] - '0';
        int d2 = buffer[3] - '0';
        int d3 = buffer[4] - '0';
        int d4 = buffer[5] - '0';
        int d5 = buffer[6] - '0';
        int d6 = buffer[7] - '0';
        int d7 = buffer[8] - '0';
        int check = ((d1 + d3 + d5) + 2 * (d2 + d4 + d6)) % 10;
        if (check != d7)
        {
            result = new ValidationResult<DeaNumberValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<DeaNumberValue>(true, new DeaNumberValue(new string(buffer)), ValidationError.None);
        return true;
    }

    /// <summary>Generates a valid DEA number into the provided destination span.</summary>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Generates a valid DEA number using the specified options.</summary>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 9)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        destination[0] = (char)('A' + rng.Next(26));
        bool business = rng.Next(2) == 0;
        destination[1] = business ? '9' : (char)('A' + rng.Next(26));
        for (int i = 2; i < 8; i++)
            destination[i] = (char)('0' + rng.Next(10));
        int d1 = destination[2] - '0';
        int d2 = destination[3] - '0';
        int d3 = destination[4] - '0';
        int d4 = destination[5] - '0';
        int d5 = destination[6] - '0';
        int d6 = destination[7] - '0';
        int check = ((d1 + d3 + d5) + 2 * (d2 + d4 + d6)) % 10;
        destination[8] = (char)('0' + check);
        written = 9;
        return true;
    }
}
