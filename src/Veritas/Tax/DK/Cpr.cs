using System;
using Veritas;

namespace Veritas.Tax.DK;

public readonly struct CprValue
{
    public string Value { get; }
    public CprValue(string value) => Value = value;
}

/// <summary>
/// Danish CPR number (10 digits, structural validation only).
/// </summary>
public static class Cpr
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<CprValue> result)
    {
        Span<char> digits = stackalloc char[10];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<CprValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 10)
        {
            result = new ValidationResult<CprValue>(false, default, ValidationError.Length);
            return true;
        }
        if (!DateTime.TryParseExact(new string(digits[..6]), "ddMMyy", null, System.Globalization.DateTimeStyles.None, out _))
        {
            result = new ValidationResult<CprValue>(false, default, ValidationError.Format);
            return true;
        }
        result = new ValidationResult<CprValue>(true, new CprValue(new string(digits)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written=0;
        if (destination.Length<10) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<char> digits = destination[..10];
        int year=rng.Next(1900,2100);
        int month=rng.Next(1,13);
        int day=rng.Next(1, DateTime.DaysInMonth(year,month)+1);
        digits[0]=(char)('0'+(day/10));
        digits[1]=(char)('0'+(day%10));
        digits[2]=(char)('0'+(month/10));
        digits[3]=(char)('0'+(month%10));
        digits[4]=(char)('0'+(year%100/10));
        digits[5]=(char)('0'+(year%10));
        for(int i=6;i<10;i++) digits[i]=(char)('0'+rng.Next(10));
        written=10;
        return true;
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

