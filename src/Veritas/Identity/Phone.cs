using System;
using Veritas;

namespace Veritas.Identity;

public readonly struct PhoneValue { public string Value { get; } public PhoneValue(string v) => Value = v; }

public static class Phone
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<PhoneValue> result)
    {
        Span<char> buf = stackalloc char[16];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            if (len == 0 && ch == '+') { buf[len++] = ch; continue; }
            if (ch < '0' || ch > '9') { result = new ValidationResult<PhoneValue>(false, default, ValidationError.Charset); return false; }
            if (len >= buf.Length) { result = new ValidationResult<PhoneValue>(false, default, ValidationError.Length); return false; }
            buf[len++] = ch;
        }
        if (len < 4 || len > 16) { result = new ValidationResult<PhoneValue>(false, default, ValidationError.Length); return false; }
        result = new ValidationResult<PhoneValue>(true, new PhoneValue(new string(buf[..len])), ValidationError.None);
        return true;
    }
}
