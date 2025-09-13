namespace Veritas.IntellectualProperty;

/// <summary>Represents a validated WIPO ST.16 patent publication number.</summary>
public readonly struct PatentPublicationNumberValue
{
    /// <summary>The normalized publication number.</summary>
    public string Value { get; }
    public PatentPublicationNumberValue(string value) => Value = value;
    public override string ToString() => Value;
}
