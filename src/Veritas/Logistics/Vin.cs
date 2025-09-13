using System;
using Veritas.Algorithms;
using Veritas;

namespace Veritas.Logistics;

public readonly struct VinValue { public string Value { get; } public VinValue(string v) => Value = v; }

public static class Vin
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<VinValue> result)
    {
        Span<char> buf = stackalloc char[17];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            if (len >= 17) { result = new ValidationResult<VinValue>(false, default, ValidationError.Length); return false; }
            buf[len++] = ch;
        }
        if (len != 17) { result = new ValidationResult<VinValue>(false, default, ValidationError.Length); return false; }
        if (!VinMap.Validate(buf)) { result = new ValidationResult<VinValue>(false, default, ValidationError.Checksum); return false; }
        result = new ValidationResult<VinValue>(true, new VinValue(new string(buf)), ValidationError.None);
        return true;
    }
}
