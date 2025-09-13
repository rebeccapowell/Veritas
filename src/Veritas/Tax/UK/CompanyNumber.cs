using System;
using Veritas;

namespace Veritas.Tax.UK;

public readonly struct CompanyNumberValue
{
    public string Value { get; }
    public CompanyNumberValue(string value) => Value = value;
}

public static class CompanyNumber
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<CompanyNumberValue> result)
    {
        Span<char> buf = stackalloc char[8];
        int len = 0;
        int letterCount = 0;
        foreach (var ch in input)
        {
            if (ch == ' ' || ch == '-') continue;
            char u = char.ToUpperInvariant(ch);
            if (letterCount < 2 && len < 2 && u >= 'A' && u <= 'Z')
            {
                buf[len++] = u;
                letterCount++;
            }
            else if (u >= '0' && u <= '9')
            {
                if (len >= 8) { result = new ValidationResult<CompanyNumberValue>(false, default, ValidationError.Length); return false; }
                buf[len++] = u;
            }
            else
            {
                result = new ValidationResult<CompanyNumberValue>(false, default, ValidationError.Charset);
                return false;
            }
        }
        if (len != 8 || (letterCount != 0 && letterCount != 2))
        {
            result = new ValidationResult<CompanyNumberValue>(false, default, ValidationError.Length);
            return false;
        }
        result = new ValidationResult<CompanyNumberValue>(true, new CompanyNumberValue(new string(buf)), ValidationError.None);
        return true;
    }
}

