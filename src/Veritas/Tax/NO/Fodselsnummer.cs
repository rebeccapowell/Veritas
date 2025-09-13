using System;
using Veritas;

namespace Veritas.Tax.NO;

public readonly struct FodselsnummerValue
{
    public string Value { get; }
    public FodselsnummerValue(string value) => Value = value;
}

/// <summary>
/// Norway Fødselsnummer (11 digits, two mod-11 checks). Supports D-numbers (day +40).
/// </summary>
public static class Fodselsnummer
{
    private static readonly int[] Weights1 = {3,7,6,1,8,9,4,5,2};
    private static readonly int[] Weights2 = {5,4,3,2,7,6,5,4,3,2};

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<FodselsnummerValue> result)
    {
        Span<char> digits = stackalloc char[11];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<FodselsnummerValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 11)
        {
            result = new ValidationResult<FodselsnummerValue>(false, default, ValidationError.Length);
            return true;
        }
        int k1 = ComputeK1(digits);
        if (k1 < 0 || k1 != digits[9] - '0')
        {
            result = new ValidationResult<FodselsnummerValue>(false, default, ValidationError.Checksum);
            return true;
        }
        int k2 = ComputeK2(digits, k1);
        if (k2 < 0 || k2 != digits[10] - '0')
        {
            result = new ValidationResult<FodselsnummerValue>(false, default, ValidationError.Checksum);
            return true;
        }
        result = new ValidationResult<FodselsnummerValue>(true, new FodselsnummerValue(new string(digits)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 11) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<char> digits = destination[..11];
        while (true)
        {
            int year = rng.Next(1900, 2100);
            int month = rng.Next(1, 13);
            int day = rng.Next(1, DateTime.DaysInMonth(year, month) + 1);
            if (rng.Next(0,2) == 1) day += 40; // D-number
            int individual = rng.Next(0, 1000);
            int yy = year % 100;
            digits[0] = (char)('0' + (day / 10));
            digits[1] = (char)('0' + (day % 10));
            digits[2] = (char)('0' + (month / 10));
            digits[3] = (char)('0' + (month % 10));
            digits[4] = (char)('0' + (yy / 10));
            digits[5] = (char)('0' + (yy % 10));
            digits[6] = (char)('0' + (individual / 100));
            digits[7] = (char)('0' + (individual / 10) % 10);
            digits[8] = (char)('0' + (individual % 10));
            int k1 = ComputeK1(digits);
            if (k1 < 0) continue;
            digits[9] = (char)('0' + k1);
            int k2 = ComputeK2(digits, k1);
            if (k2 < 0) continue;
            digits[10] = (char)('0' + k2);
            written = 11;
            return true;
        }
    }

    private static int ComputeK1(ReadOnlySpan<char> digits)
    {
        int sum = 0;
        for (int i=0;i<9;i++) sum += (digits[i]-'0')*Weights1[i];
        int r = 11 - sum%11;
        if (r == 11) return 0;
        if (r == 10) return -1;
        return r;
    }

    private static int ComputeK2(ReadOnlySpan<char> digits, int k1)
    {
        int sum = 0;
        for (int i=0;i<9;i++) sum += (digits[i]-'0')*Weights2[i];
        sum += k1*2;
        int r = 11 - sum%11;
        if (r == 11) return 0;
        if (r == 10) return -1;
        return r;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        foreach (var ch in input)
        {
            if (ch==' '||ch=='-') continue;
            if (!char.IsDigit(ch) || len>=dest.Length)
            {
                len = 0; return false;
            }
            dest[len++]=ch;
        }
        return true;
    }
}

