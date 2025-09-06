using System;
using Veritas;

namespace Veritas.Identity;

public readonly struct EthereumValue
{
    public string Value { get; }
    public EthereumValue(string value) => Value = value;
}

/// <summary>Provides validation for Ethereum addresses.</summary>
public static class Ethereum
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<EthereumValue> result)
    {
        if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            input = input[2..];
        if (input.Length != 40)
        {
            result = new ValidationResult<EthereumValue>(false, default, ValidationError.Length);
            return true;
        }
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            if (!Uri.IsHexDigit(c))
            {
                result = new ValidationResult<EthereumValue>(false, default, ValidationError.Charset);
                return true;
            }
        }
        string value = "0x" + new string(input).ToLowerInvariant();
        result = new ValidationResult<EthereumValue>(true, new EthereumValue(value), ValidationError.None);
        return true;
    }
}
