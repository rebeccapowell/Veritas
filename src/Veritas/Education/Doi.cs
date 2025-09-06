using System;
using Veritas;

namespace Veritas.Education;

public readonly struct DoiValue { public string Value { get; } public DoiValue(string v) => Value = v; }

public static class Doi
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<DoiValue> result)
    {
        string s = new string(input).Trim();
        if (!s.StartsWith("10.")) { result = new ValidationResult<DoiValue>(false, default, ValidationError.Format); return true; }
        int slash = s.IndexOf('/');
        if (slash <= 3 || slash == s.Length - 1) { result = new ValidationResult<DoiValue>(false, default, ValidationError.Format); return true; }
        for (int i = 3; i < slash; i++) if (!char.IsDigit(s[i])) { result = new ValidationResult<DoiValue>(false, default, ValidationError.Charset); return true; }
        result = new ValidationResult<DoiValue>(true, new DoiValue(s), ValidationError.None);
        return true;
    }
}
