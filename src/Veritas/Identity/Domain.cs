using System;
using Veritas;

namespace Veritas.Identity;

public readonly struct DomainValue { public string Value { get; } public DomainValue(string v) => Value = v; }

public static class Domain
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<DomainValue> result)
    {
        Span<char> buf = stackalloc char[253];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ') { result = new ValidationResult<DomainValue>(false, default, ValidationError.Charset); return false; }
            if (len >= buf.Length) { result = new ValidationResult<DomainValue>(false, default, ValidationError.Length); return false; }
            buf[len++] = char.ToLowerInvariant(ch);
        }
        if (len == 0 || len > 253) { result = new ValidationResult<DomainValue>(false, default, ValidationError.Length); return false; }
        int labelLen = 0;
        bool lastAlpha = false;
        for (int i = 0; i < len; i++)
        {
            char c = buf[i];
            if (c == '.')
            {
                if (labelLen == 0 || labelLen > 63 || !lastAlpha) { result = new ValidationResult<DomainValue>(false, default, ValidationError.Format); return false; }
                labelLen = 0; lastAlpha = false; continue;
            }
            if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || (c == '-' && labelLen > 0))
            {
                lastAlpha = (c >= 'a' && c <= 'z');
                labelLen++;
            }
            else { result = new ValidationResult<DomainValue>(false, default, ValidationError.Charset); return false; }
        }
        if (labelLen == 0 || labelLen > 63 || !lastAlpha) { result = new ValidationResult<DomainValue>(false, default, ValidationError.Format); return false; }
        result = new ValidationResult<DomainValue>(true, new DomainValue(new string(buf[..len])), ValidationError.None);
        return true;
    }
}
