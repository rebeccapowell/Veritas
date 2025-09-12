namespace Veritas.Identity.India;

/// <summary>Represents a validated Indian Aadhaar number.</summary>
public readonly struct AadhaarValue
{
    /// <summary>Gets the normalized Aadhaar string.</summary>
    public string Value { get; }

    /// <summary>Creates a new <see cref="AadhaarValue"/>.</summary>
    public AadhaarValue(string value) => Value = value;

    public override string ToString() => Value;
}
