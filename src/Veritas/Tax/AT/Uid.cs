using System;
using Veritas;

namespace Veritas.Tax.AT;

public readonly struct UidValue
{
    public string Value { get; }
    public UidValue(string value) => Value = value;
}

/// <summary>
/// Austrian VAT UID (ATU + 8 digits, weighted mod-10).
/// </summary>
public static class Uid
{
    private static readonly int[] Weights = {1,2,1,2,1,2,1};

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<UidValue> result)
    {
        Span<char> digits = stackalloc char[8];
        if (!Normalize(input, digits, out int len))
        {
            result = new ValidationResult<UidValue>(false, default, ValidationError.Format);
            return true;
        }
        if (len != 8)
        {
            result = new ValidationResult<UidValue>(false, default, ValidationError.Length);
            return true;
        }
        int check = ComputeCheckDigit(digits[..7]);
        if (digits[7] - '0' != check)
        {
            result = new ValidationResult<UidValue>(false, default, ValidationError.Checksum);
            return true;
        }
        result = new ValidationResult<UidValue>(true, new UidValue("ATU" + new string(digits)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        written = 0;
        if (destination.Length < 11) return false; // ATU + 8 digits
        destination[0] = 'A';
        destination[1] = 'T';
        destination[2] = 'U';
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<char> digits = destination.Slice(3,8);
        for (int i=0;i<7;i++)
            digits[i] = (char)('0'+rng.Next(10));
        digits[7] = (char)('0'+ComputeCheckDigit(digits[..7]));
        written = 11;
        return true;
    }

    private static int ComputeCheckDigit(ReadOnlySpan<char> digits)
    {
        int sum=0;
        for(int i=0;i<Weights.Length;i++)
        {
            int prod = (digits[i]-'0')*Weights[i];
            sum += prod/10 + prod%10;
        }
        sum += 4; // constant per spec
        int check = 10 - (sum%10);
        return check%10;
    }

    private static bool Normalize(ReadOnlySpan<char> input, Span<char> dest, out int len)
    {
        len = 0;
        int i = 0;
        if (input.Length >=3 && (input[0]=='A'||input[0]=='a') && (input[1]=='T'||input[1]=='t') && (input[2]=='U'||input[2]=='u'))
            i = 3;
        for (; i < input.Length; i++)
        {
            char ch = input[i];
            if (ch==' '||ch=='-') continue;
            if (!char.IsDigit(ch)) { len = 0; return false; }
            if (len>=dest.Length){ len=0; return false; }
            dest[len++] = ch;
        }
        return len>0;
    }
}

