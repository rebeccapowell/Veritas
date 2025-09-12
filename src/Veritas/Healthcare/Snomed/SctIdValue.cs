namespace Veritas.Healthcare.Snomed;

/// <summary>Represents a validated SNOMED CT Concept ID.</summary>
public readonly struct SctIdValue
{
    public string Value { get; }
    public SctIdValue(string value) => Value = value;
    public override string ToString() => Value;
}
