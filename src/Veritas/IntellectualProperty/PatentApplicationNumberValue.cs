namespace Veritas.IntellectualProperty;

/// <summary>Represents a validated WIPO ST.13 patent application number.</summary>
public readonly struct PatentApplicationNumberValue
{
    /// <summary>The normalized application number.</summary>
    public string Value { get; }
    public PatentApplicationNumberValue(string value) => Value = value;
    public override string ToString() => Value;
}
