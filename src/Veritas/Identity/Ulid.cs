using System;
using Veritas;

namespace Veritas.Identity;

public readonly struct UlidValue { public string Value { get; } public UlidValue(string v) => Value = v; }

public static class Ulid
{
    private const string Alphabet = "0123456789ABCDEFGHJKMNPQRSTVWXYZ"; // Crockford base32 without I,L,O,U

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<UlidValue> result)
    {
        Span<char> buf = stackalloc char[26];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            char u = char.ToUpperInvariant(ch);
            if (Alphabet.IndexOf(u) < 0) { result = new ValidationResult<UlidValue>(false, default, ValidationError.Charset); return true; }
            if (len >= 26) { result = new ValidationResult<UlidValue>(false, default, ValidationError.Length); return true; }
            buf[len++] = u;
        }
        if (len != 26) { result = new ValidationResult<UlidValue>(false, default, ValidationError.Length); return true; }
        result = new ValidationResult<UlidValue>(true, new UlidValue(new string(buf)), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        if (destination.Length < 26)
        {
            written = 0;
            return false;
        }
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        for (int i = 9; i >= 0; i--)
        {
            destination[i] = Alphabet[(int)(timestamp & 31)];
            timestamp >>= 5;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        Span<byte> random = stackalloc byte[10];
        rng.NextBytes(random);
        int idx = 10;
        int bitBuffer = 0;
        int bitsInBuffer = 0;
        for (int i = 0; i < random.Length; i++)
        {
            bitBuffer = (bitBuffer << 8) | random[i];
            bitsInBuffer += 8;
            while (bitsInBuffer >= 5)
            {
                bitsInBuffer -= 5;
                destination[idx++] = Alphabet[(bitBuffer >> bitsInBuffer) & 31];
                bitBuffer &= (1 << bitsInBuffer) - 1;
            }
        }
        written = 26;
        return true;
    }
}
