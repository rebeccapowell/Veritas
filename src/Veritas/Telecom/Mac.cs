using System;
using Veritas;

namespace Veritas.Telecom;

public readonly struct MacValue { public string Value { get; } public MacValue(string v) => Value = v; }

public static class Mac
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<MacValue> result)
    {
        Span<char> buf = stackalloc char[12];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == '-' || ch == ':' || ch == '.') continue;
            char c = char.ToUpperInvariant(ch);
            if (!Uri.IsHexDigit(c)) { result = new ValidationResult<MacValue>(false, default, ValidationError.Charset); return true; }
            if (len >= 12) { result = new ValidationResult<MacValue>(false, default, ValidationError.Length); return true; }
            buf[len++] = c;
        }
        if (len != 12) { result = new ValidationResult<MacValue>(false, default, ValidationError.Length); return true; }
        result = new ValidationResult<MacValue>(true, new MacValue(new string(buf)), ValidationError.None);
        return true;
    }
}
