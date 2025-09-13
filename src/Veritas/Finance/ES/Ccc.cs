using System;
using Veritas;

namespace Veritas.Finance.ES;

/// <summary>Spanish bank account code (CCC) consisting of bank, branch, control digits, and account number.</summary>
public readonly struct CccValue
{
    /// <summary>The normalized CCC string.</summary>
    public string Value { get; }
    public CccValue(string value) => Value = value;
}

/// <summary>Provides validation and generation for Spanish CCC numbers.</summary>
public static class Ccc
{
    /// <summary>Attempts to validate the supplied CCC.</summary>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<CccValue> result)
    {
        Span<char> digits = stackalloc char[20];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            if (ch < '0' || ch > '9' || len >= 20)
            {
                result = new ValidationResult<CccValue>(false, default, ValidationError.Charset);
                return false;
            }
            digits[len++] = ch;
        }
        if (len != 20)
        {
            result = new ValidationResult<CccValue>(false, default, ValidationError.Length);
            return false;
        }
        int cd1 = digits[8] - '0';
        int cd2 = digits[9] - '0';
        if (ComputeBankBranch(digits[..8]) != cd1 || ComputeAccount(digits[10..]) != cd2)
        {
            result = new ValidationResult<CccValue>(false, default, ValidationError.Checksum);
            return false;
        }
        result = new ValidationResult<CccValue>(true, new CccValue(new string(digits)), ValidationError.None);
        return true;
    }

    /// <summary>Generates a valid CCC into the provided destination span.</summary>
    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    /// <summary>Generates a valid CCC using the specified options.</summary>
    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 20)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < 8; i++) destination[i] = (char)('0' + rng.Next(10));
        for (int i = 10; i < 20; i++) destination[i] = (char)('0' + rng.Next(10));
        destination[8] = (char)('0' + ComputeBankBranch(destination[..8]));
        destination[9] = (char)('0' + ComputeAccount(destination[10..]));
        written = 20;
        return true;
    }

    private static int ComputeBankBranch(ReadOnlySpan<char> span)
    {
        int[] weights = { 4, 8, 5, 10, 9, 7, 3, 6 };
        int sum = 0;
        for (int i = 0; i < 8; i++) sum += (span[i] - '0') * weights[i];
        int mod = 11 - (sum % 11);
        if (mod == 10) return 1;
        if (mod == 11) return 0;
        return mod;
    }

    private static int ComputeAccount(ReadOnlySpan<char> span)
    {
        int[] weights = { 1, 2, 4, 8, 5, 10, 9, 7, 3, 6 };
        int sum = 0;
        for (int i = 0; i < 10; i++) sum += (span[i] - '0') * weights[i];
        int mod = 11 - (sum % 11);
        if (mod == 10) return 1;
        if (mod == 11) return 0;
        return mod;
    }
}

