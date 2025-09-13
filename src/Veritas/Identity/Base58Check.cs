using System;
using Veritas;
using Veritas.Algorithms;

namespace Veritas.Identity;

public readonly struct Base58CheckValue
{
    public string Value { get; }
    public Base58CheckValue(string value) => Value = value;
}

public static class Base58Check
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<Base58CheckValue> result)
    {
        Span<byte> buffer = stackalloc byte[input.Length];
        if (Algorithms.Base58Check.TryDecode(input, buffer, out _))
        {
            result = new ValidationResult<Base58CheckValue>(true, new Base58CheckValue(new string(input)), ValidationError.None);
            return true;
        }
        result = new ValidationResult<Base58CheckValue>(false, default, ValidationError.Format);
        return false;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<byte> data = stackalloc byte[20];
        rng.NextBytes(data);
        return Algorithms.Base58Check.TryEncode(data, destination, out written);
    }
}
