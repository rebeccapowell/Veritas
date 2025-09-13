using System;
using Veritas;

namespace Veritas.Tax.EE;

public readonly struct IsikukoodValue
{
    public string Value { get; }
    public IsikukoodValue(string value) => Value = value;
}

/// <summary>
/// Estonian Isikukood (11 digits, two-stage mod-11).
/// </summary>
public static class Isikukood
{
    private static readonly int[] Weights1 = {1,2,3,4,5,6,7,8,9,1};
    private static readonly int[] Weights2 = {3,4,5,6,7,8,9,1,2,3};

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<IsikukoodValue> result)
    {
        Span<char> digits = stackalloc char[11];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<IsikukoodValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 11)
        {
            result = new ValidationResult<IsikukoodValue>(false, default, ValidationError.Length);
            return true;
        }
        int check = ComputeCheckDigit(digits[..10]);
        if (digits[10]-'0' != check)
        {
            result = new ValidationResult<IsikukoodValue>(false, default, ValidationError.Checksum);
            return true;
        }
        result = new ValidationResult<IsikukoodValue>(true, new IsikukoodValue(new string(digits)), ValidationError.None);
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
        int s = year>=2000? (rng.Next(2)==0?5:6) : (rng.Next(2)==0?3:4);
        digits[0]=(char)('0'+s);
        digits[1]=(char)('0'+(year%100/10));
        digits[2]=(char)('0'+(year%10));
        digits[3]=(char)('0'+(month/10));
        digits[4]=(char)('0'+(month%10));
        digits[5]=(char)('0'+(day/10));
        digits[6]=(char)('0'+(day%10));
        for(int i=7;i<10;i++) digits[i]=(char)('0'+rng.Next(10));
        digits[10]=(char)('0'+ComputeCheckDigit(digits[..10]));
        written=11;
        return true;
    }

    private static int ComputeCheckDigit(ReadOnlySpan<char> digits)
    {
        int sum=0;
        for(int i=0;i<Weights1.Length;i++) sum += (digits[i]-'0')*Weights1[i];
        int r=sum%11;
        if (r<10) return r;
        sum=0;
        for(int i=0;i<Weights2.Length;i++) sum += (digits[i]-'0')*Weights2[i];
        r=sum%11;
        return r<10? r : 0;
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

