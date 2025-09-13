using System;
using Veritas;

namespace Veritas.Tax.PE;

public readonly struct RucValue
{
    public string Value { get; }
    public RucValue(string value) => Value = value;
}

/// <summary>
/// Peru RUC tax identifier (11 digits, weighted checksum).
/// </summary>
public static class Ruc
{
    private static readonly int[] Weights = {5,4,3,2,7,6,5,4,3,2};

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<RucValue> result)
    {
        Span<char> digits = stackalloc char[11];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<RucValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 11)
        {
            result = new ValidationResult<RucValue>(false, default, ValidationError.Length);
            return true;
        }
        int check = ComputeCheckDigit(digits[..10]);
        if (digits[10]-'0' != check)
        {
            result = new ValidationResult<RucValue>(false, default, ValidationError.Checksum);
            return true;
        }
        result = new ValidationResult<RucValue>(true, new RucValue(new string(digits)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written=0;
        if(destination.Length<11) return false;
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<char> digits = destination[..11];
        for(int i=0;i<10;i++) digits[i]=(char)('0'+rng.Next(10));
        digits[10]=(char)('0'+ComputeCheckDigit(digits[..10]));
        written=11;
        return true;
    }

    private static int ComputeCheckDigit(ReadOnlySpan<char> digits)
    {
        int sum=0;
        for(int i=0;i<Weights.Length;i++) sum += (digits[i]-'0')*Weights[i];
        int r = 11 - (sum % 11);
        if (r==10) return 0;
        if (r==11) return 1;
        return r;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len=0;
        foreach(var ch in input)
        {
            if (ch==' '||ch=='-') continue;
            if(!char.IsDigit(ch)){ len=0; return false; }
            if(len>=dest.Length){ len=0; return false; }
            dest[len++]=ch;
        }
        return true;
    }
}

