using System;
using Veritas;

namespace Veritas.Logistics;

/// <summary>UPU S10 international postal tracking number.</summary>
public readonly struct UpuS10Value
{
    /// <summary>The normalized S10 string.</summary>
    public string Value { get; }
    public UpuS10Value(string value) => Value = value;
}

/// <summary>Provides validation and generation for UPU S10 tracking numbers.</summary>
public static class UpuS10
{
    /// <summary>Validates the supplied S10 tracking number.</summary>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<UpuS10Value> result)
    {
        Span<char> buf = stackalloc char[13];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            if (len >= 13) { result = new ValidationResult<UpuS10Value>(false, default, ValidationError.Length); return false; }
            char c = ch;
            if (len < 2 || len >= 11)
            {
                if (c >= 'a' && c <= 'z') c = (char)(c - 32);
                if (c < 'A' || c > 'Z') { result = new ValidationResult<UpuS10Value>(false, default, ValidationError.Charset); return false; }
            }
            else
            {
                if (c < '0' || c > '9') { result = new ValidationResult<UpuS10Value>(false, default, ValidationError.Charset); return false; }
            }
            buf[len++] = c;
        }
        if (len != 13)
        {
            result = new ValidationResult<UpuS10Value>(false, default, ValidationError.Length);
            return false;
        }
        int check = buf[10] - '0';
        int expected = ComputeCheckDigit(buf.Slice(2, 8));
        if (check != expected)
        {
            result = new ValidationResult<UpuS10Value>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<UpuS10Value>(true, new UpuS10Value(new string(buf)), ValidationError.None);
        return true;
    }

    /// <summary>Generates a valid S10 tracking number into the provided destination span.</summary>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Generates a valid S10 tracking number using the specified options.</summary>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 13)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        var svc = _serviceCodes[rng.Next(_serviceCodes.Length)];
        destination[0] = svc[0];
        destination[1] = svc[1];
        for (int i = 0; i < 8; i++) destination[2 + i] = (char)('0' + rng.Next(10));
        destination[10] = (char)('0' + ComputeCheckDigit(destination.Slice(2, 8)));
        var cc = _countryCodes[rng.Next(_countryCodes.Length)];
        destination[11] = cc[0];
        destination[12] = cc[1];
        written = 13;
        return true;
    }

    private static int ComputeCheckDigit(ReadOnlySpan<char> digits)
    {
        int[] weights = { 8, 6, 4, 2, 3, 5, 9, 7 };
        int sum = 0;
        for (int i = 0; i < 8; i++) sum += (digits[i] - '0') * weights[i];
        int mod = 11 - (sum % 11);
        if (mod == 10) return 0;
        if (mod == 11) return 5;
        return mod;
    }

    private static readonly string[] _serviceCodes = { "RA", "RB", "RC", "RR", "CP", "LX" };
    private static readonly string[] _countryCodes = { "US", "GB", "DE", "FR", "CN", "JP", "BR", "IN" };
}

