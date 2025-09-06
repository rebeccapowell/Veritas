using System;
using System.Globalization;
using Veritas;

namespace Veritas.Telecom;

public readonly struct AsnValue { public uint Value { get; } public AsnValue(uint v) => Value = v; }

public static class Asn
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<AsnValue> result)
    {
        if (uint.TryParse(input, NumberStyles.None, CultureInfo.InvariantCulture, out var value))
        {
            result = new ValidationResult<AsnValue>(true, new AsnValue(value), ValidationError.None);
            return true;
        }
        result = new ValidationResult<AsnValue>(false, default, ValidationError.Format);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        uint value = (uint)rng.NextInt64(0, (long)uint.MaxValue + 1);
        var s = value.ToString(CultureInfo.InvariantCulture);
        if (destination.Length < s.Length) { written = 0; return false; }
        s.AsSpan().CopyTo(destination);
        written = s.Length;
        return true;
    }
}
