namespace Veritas;

/// <summary>Represents the outcome of validating an identifier.</summary>
/// <typeparam name="T">Strongly typed value when validation succeeds.</typeparam>
public readonly struct ValidationResult<T>
{
    /// <summary>Gets a value indicating whether the input was valid.</summary>
    public bool IsValid { get; }

    /// <summary>Gets the parsed value when <see cref="IsValid"/> is <c>true</c>.</summary>
    public T? Value { get; }

    /// <summary>Gets the classification of any validation error.</summary>
    public ValidationError Error { get; }

    /// <summary>Gets an optional message describing the validation result.</summary>
    public string? Message { get; }

    /// <summary>Initializes a new instance of the <see cref="ValidationResult{T}"/> struct.</summary>
    /// <param name="isValid">Indicates whether the input was valid.</param>
    /// <param name="value">The parsed value when validation succeeds.</param>
    /// <param name="error">The error classification.</param>
    /// <param name="message">Optional human-readable message.</param>
    public ValidationResult(bool isValid, T? value, ValidationError error, string? message = null)
    {
        IsValid = isValid;
        Value = value;
        Error = error;
        Message = message;
    }
}
