using System;
using Veritas;

namespace Veritas.Identity;

public readonly struct NanoIdValue { public string Value { get; } public NanoIdValue(string v) => Value = v; }

public static class NanoId
{
    private const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz-";

    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<NanoIdValue> result)
    {
        Span<char> buf = stackalloc char[64];
        int len = 0;
        foreach (var ch in input)
        {
            if (Alphabet.IndexOf(ch) < 0) { result = new ValidationResult<NanoIdValue>(false, default, ValidationError.Charset); return false; }
            if (len >= buf.Length) { result = new ValidationResult<NanoIdValue>(false, default, ValidationError.Length); return false; }
            buf[len++] = ch;
        }
        if (len != 21) { result = new ValidationResult<NanoIdValue>(false, default, ValidationError.Length); return false; }
        result = new ValidationResult<NanoIdValue>(true, new NanoIdValue(new string(buf[..len])), ValidationError.None);
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
        => TryGenerate(default, destination, out written);

    public static bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written)
    {
        const int length = 21;
        if (destination.Length < length)
        {
            written = 0;
            return false;
        }
        var rng = options.Seed.HasValue ? new Random(options.Seed.Value) : Random.Shared;
        for (int i = 0; i < length; i++)
            destination[i] = Alphabet[rng.Next(Alphabet.Length)];
        written = length;
        return true;
    }
}
