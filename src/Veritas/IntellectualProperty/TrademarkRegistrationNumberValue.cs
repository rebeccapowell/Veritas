namespace Veritas.IntellectualProperty;

/// <summary>Represents a validated trademark registration number.</summary>
public readonly struct TrademarkRegistrationNumberValue
{
    /// <summary>The normalized registration number.</summary>
    public string Value { get; }
    public TrademarkRegistrationNumberValue(string value) => Value = value;
    public override string ToString() => Value;
}
