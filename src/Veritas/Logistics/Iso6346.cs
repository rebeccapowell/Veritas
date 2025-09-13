using System;
using Veritas.Algorithms;
using Veritas;

namespace Veritas.Logistics;

public readonly struct Iso6346Value { public string Value { get; } public Iso6346Value(string v) => Value = v; }

public static class Iso6346
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<Iso6346Value> result)
    {
        Span<char> buf = stackalloc char[11];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ') continue;
            if (len >= 11) { result = new ValidationResult<Iso6346Value>(false, default, ValidationError.Length); return false; }
            buf[len++] = char.ToUpperInvariant(ch);
        }
        if (len != 11) { result = new ValidationResult<Iso6346Value>(false, default, ValidationError.Length); return false; }
        if (!Iso6346Algorithm.Validate(buf)) { result = new ValidationResult<Iso6346Value>(false, default, ValidationError.Checksum); return false; }
        result = new ValidationResult<Iso6346Value>(true, new Iso6346Value(new string(buf)), ValidationError.None);
        return true;
    }
}
