namespace Veritas.Legal.EU;

/// <summary>Represents a validated European Patent Office publication identifier.</summary>
public readonly struct EuropeanPatentOfficePublicationIdValue
{
    /// <summary>The normalized publication identifier.</summary>
    public string Value { get; }
    public EuropeanPatentOfficePublicationIdValue(string value) => Value = value;
    public override string ToString() => Value;
}
