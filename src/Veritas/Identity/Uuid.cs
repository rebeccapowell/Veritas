using System;
using Veritas;

namespace Veritas.Identity;

/// <summary>Represents a validated UUID/GUID.</summary>
public readonly struct UuidValue
{
    public Guid Value { get; }
    public UuidValue(Guid value) => Value = value;
}

/// <summary>Provides validation for UUID strings.</summary>
public static class Uuid
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<UuidValue> result)
    {
        var str = input.ToString();
        if (Guid.TryParse(str, out var guid))
        {
            result = new ValidationResult<UuidValue>(true, new UuidValue(guid), ValidationError.None);
        }
        else
        {
            result = new ValidationResult<UuidValue>(false, default, ValidationError.Format);
        }
        return true;
    }

    public static bool TryGenerate(Span<char> destination, out int written)
    {
        var g = Guid.NewGuid();
        var s = g.ToString("D");
        if (destination.Length < s.Length)
        {
            written = 0;
            return false;
        }
        s.AsSpan().CopyTo(destination);
        written = s.Length;
        return true;
    }
}

