using System;
namespace Veritas;

/// <summary>Validates an identifier into a strongly typed value.</summary>
/// <typeparam name="T">Resulting strong type.</typeparam>
public interface IValidator<T>
{
    /// <summary>Attempts to validate the provided input.</summary>
    /// <param name="input">The value to examine.</param>
    /// <param name="result">The validation result when the method returns.</param>
    /// <returns><c>true</c> if the input could be processed; otherwise, <c>false</c>.</returns>
    bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<T> result);
}
