namespace Veritas;

/// <summary>Represents the outcome of validating an identifier.</summary>
/// <typeparam name="T">Strongly typed value when validation succeeds.</typeparam>
public readonly struct ValidationResult<T>
{
    public bool IsValid { get; }
    public T? Value { get; }
    public ValidationError Error { get; }
    public string? Message { get; }

    public ValidationResult(bool isValid, T? value, ValidationError error, string? message = null)
    {
        IsValid = isValid;
        Value = value;
        Error = error;
        Message = message;
    }
}
