namespace Veritas.IntellectualProperty;

/// <summary>Represents a validated International Standard Musical Work Code (ISWC).</summary>
public readonly struct IswcValue
{
    /// <summary>The normalized ISWC string.</summary>
    public string Value { get; }
    public IswcValue(string value) => Value = value;
    public override string ToString() => Value;
}
