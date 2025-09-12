namespace Veritas.IP.Singapore;

/// <summary>Represents a validated Singapore IPOS application number.</summary>
public readonly struct IpApplicationNumberValue
{
    public string Value { get; }
    public IpApplicationNumberValue(string value) => Value = value;
    public override string ToString() => Value;
}
