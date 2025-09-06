using System;
namespace Veritas;

/// <summary>Validates an identifier into a strongly typed value.</summary>
/// <typeparam name="T">Resulting strong type.</typeparam>
public interface IValidator<T>
{
    bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<T> result);
}
