using System;
using Veritas;

namespace Veritas.Tax.UK;

public readonly struct NinoValue { public string Value { get; } public NinoValue(string v) => Value = v; }

public static class Nino
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<NinoValue> result)
    {
        Span<char> buf = stackalloc char[9];
        int len = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            char u = char.ToUpperInvariant(ch);
            if (len < 2)
            {
                if (u < 'A' || u > 'Z') { result = new ValidationResult<NinoValue>(false, default, ValidationError.Charset); return true; }
            }
            else if (len < 8)
            {
                if (u < '0' || u > '9') { result = new ValidationResult<NinoValue>(false, default, ValidationError.Charset); return true; }
            }
            else
            {
                if (!(u == 'A' || u == 'B' || u == 'C' || u == 'D' || u == ' ')) { result = new ValidationResult<NinoValue>(false, default, ValidationError.Charset); return true; }
            }
            if (len >= 9) { result = new ValidationResult<NinoValue>(false, default, ValidationError.Length); return true; }
            buf[len++] = u;
        }
        if (len < 8 || len > 9) { result = new ValidationResult<NinoValue>(false, default, ValidationError.Length); return true; }
        if ("DFIQUV".IndexOf(buf[0]) >= 0 || "DFIQUVO".IndexOf(buf[1]) >= 0) { result = new ValidationResult<NinoValue>(false, default, ValidationError.CountryRule); return true; }
        result = new ValidationResult<NinoValue>(true, new NinoValue(new string(buf[..len])), ValidationError.None);
        return true;
    }
}
