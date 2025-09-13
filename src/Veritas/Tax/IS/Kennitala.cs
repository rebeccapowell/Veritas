using System;
using Veritas;

namespace Veritas.Tax.IS;

public readonly struct KennitalaValue
{
    public string Value { get; }
    public KennitalaValue(string value) => Value = value;
}

/// <summary>
/// Icelandic Kennitala (10 digits, mod-11 checksum).
/// </summary>
public static class Kennitala
{
    private static readonly int[] Weights = {3,2,7,6,5,4,3,2};

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<KennitalaValue> result)
    {
        Span<char> digits = stackalloc char[10];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<KennitalaValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 10)
        {
            result = new ValidationResult<KennitalaValue>(false, default, ValidationError.Length);
            return true;
        }
        if (digits[9] != '0' && digits[9] != '9')
        {
            result = new ValidationResult<KennitalaValue>(false, default, ValidationError.Format);
            return true;
        }
        int check = ComputeCheckDigit(digits);
        if (digits[8]-'0' != check)
        {
            result = new ValidationResult<KennitalaValue>(false, default, ValidationError.Checksum);
            return true;
        }
        result = new ValidationResult<KennitalaValue>(true, new KennitalaValue(new string(digits)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written=0;
        if (destination.Length <10) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<char> digits = destination[..10];
        int year = rng.Next(1900,2100);
        int month = rng.Next(1,13);
        int day = rng.Next(1, DateTime.DaysInMonth(year,month)+1);
        digits[0]=(char)('0'+(day/10));
        digits[1]=(char)('0'+(day%10));
        digits[2]=(char)('0'+(month/10));
        digits[3]=(char)('0'+(month%10));
        digits[4]=(char)('0'+(year%100/10));
        digits[5]=(char)('0'+(year%10));
        digits[6]=(char)('0'+rng.Next(10));
        digits[7]=(char)('0'+rng.Next(10));
        digits[8]=(char)('0'+ComputeCheckDigit(digits));
        digits[9]=(char)('0'+(year>=2000?0:9));
        written=10;
        return true;
    }

    private static int ComputeCheckDigit(ReadOnlySpan<char> digits)
    {
        int sum=0;
        for(int i=0;i<Weights.Length;i++)
            sum += (digits[i]-'0')*Weights[i];
        int r = 11 - (sum%11);
        if (r==11) return 0;
        if (r==10) return -1; // invalid
        return r;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len=0;
        foreach(var ch in input)
        {
            if (ch=='-'||ch==' ') continue;
            if(!char.IsDigit(ch)) { len=0; return false; }
            if (len>=dest.Length){ len=0; return false; }
            dest[len++]=ch;
        }
        return true;
    }
}

