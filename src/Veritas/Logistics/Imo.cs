using System;
using Veritas;

namespace Veritas.Logistics;

public readonly struct ImoValue { public string Value { get; } public ImoValue(string v) => Value = v; }

public static class Imo
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<ImoValue> result)
    {
        Span<char> digits = stackalloc char[7];
        int len = 0;
        int idx = 0;
        if (input.Length >= 3 && char.ToUpperInvariant(input[0]) == 'I' && char.ToUpperInvariant(input[1]) == 'M' && char.ToUpperInvariant(input[2]) == 'O')
            idx = 3;
        for (; idx < input.Length; idx++)
        {
            char ch = input[idx];
            if (ch == ' ') continue;
            if ((uint)(ch - '0') > 9)
            {
                result = new ValidationResult<ImoValue>(false, default, ValidationError.Charset);
                return false;
            }
            if (len >= 7)
            {
                result = new ValidationResult<ImoValue>(false, default, ValidationError.Length);
                return false;
            }
            digits[len++] = ch;
        }
        if (len != 7)
        {
            result = new ValidationResult<ImoValue>(false, default, ValidationError.Length);
            return false;
        }
        int sum = 0;
        for (int i = 0; i < 6; i++)
            sum += (digits[i] - '0') * (7 - i);
        int check = sum % 10;
        if (digits[6] - '0' != check)
        {
            result = new ValidationResult<ImoValue>(false, default, ValidationError.Checksum);
            return false;
        }
        Span<char> norm = stackalloc char[10];
        norm[0] = 'I';
        norm[1] = 'M';
        norm[2] = 'O';
        digits.CopyTo(norm[3..]);
        result = new ValidationResult<ImoValue>(true, new ImoValue(new string(norm)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 10)
        {
            written = 0;
            return false;
        }
        destination[0] = 'I';
        destination[1] = 'M';
        destination[2] = 'O';
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        int sum = 0;
        for (int i = 0; i < 6; i++)
        {
            int d = rng.Next(10);
            destination[3 + i] = (char)('0' + d);
            sum += d * (7 - i);
        }
        destination[9] = (char)('0' + (sum % 10));
        written = 10;
        return true;
    }
}

