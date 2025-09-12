namespace Veritas.Identity.Luxembourg;

/// <summary>Represents a validated Luxembourg National ID.</summary>
public readonly struct NationalIdValue
{
    public string Value { get; }
    public NationalIdValue(string value) => Value = value;
    public override string ToString() => Value;
}
