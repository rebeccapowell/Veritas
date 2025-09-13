namespace Veritas.Legal.EU;

/// <summary>Represents a validated European Case Law Identifier (ECLI).</summary>
public readonly struct EcliValue
{
    /// <summary>The normalized ECLI string.</summary>
    public string Value { get; }
    public EcliValue(string value) => Value = value;
    public override string ToString() => Value;
}
