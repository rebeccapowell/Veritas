using System;
using Veritas;

namespace Veritas.Tax.LV;

public readonly struct PersonasKodsValue
{
    public string Value { get; }
    public PersonasKodsValue(string value) => Value = value;
}

/// <summary>
/// Latvian Personas kods (11 digits, mod-11 checksum).
/// </summary>
public static class PersonasKods
{
    private static readonly int[] Weights = {1,6,3,7,9,10,5,8,4,2};

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<PersonasKodsValue> result)
    {
        Span<char> digits = stackalloc char[11];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<PersonasKodsValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 11)
        {
            result = new ValidationResult<PersonasKodsValue>(false, default, ValidationError.Length);
            return true;
        }
        int check = ComputeCheckDigit(digits[..10]);
        if (digits[10]-'0' != check)
        {
            result = new ValidationResult<PersonasKodsValue>(false, default, ValidationError.Checksum);
            return true;
        }
        result = new ValidationResult<PersonasKodsValue>(true, new PersonasKodsValue(new string(digits)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written=0;
        if (destination.Length<11) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<char> digits = destination[..11];
        int year=rng.Next(1900,2100);
        int month=rng.Next(1,13);
        int day=rng.Next(1, DateTime.DaysInMonth(year,month)+1);
        digits[0]=(char)('0'+(day/10));
        digits[1]=(char)('0'+(day%10));
        digits[2]=(char)('0'+(month/10));
        digits[3]=(char)('0'+(month%10));
        digits[4]=(char)('0'+(year%100/10));
        digits[5]=(char)('0'+(year%10));
        digits[6]=(char)('0'+rng.Next(10));
        digits[7]=(char)('0'+rng.Next(10));
        digits[8]=(char)('0'+rng.Next(10));
        digits[9]=(char)('0'+rng.Next(10));
        digits[10]=(char)('0'+ComputeCheckDigit(digits[..10]));
        written=11;
        return true;
    }

    private static int ComputeCheckDigit(ReadOnlySpan<char> digits)
    {
        int sum=0;
        for(int i=0;i<Weights.Length;i++) sum += (digits[i]-'0')*Weights[i];
        int r = (sum + 1) % 11;
        return r == 10 ? 0 : r;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len=0;
        foreach(var ch in input)
        {
            if (ch=='-'||ch==' ') continue;
            if(!char.IsDigit(ch)){ len=0; return false; }
            if(len>=dest.Length){ len=0; return false; }
            dest[len++]=ch;
        }
        return true;
    }
}

