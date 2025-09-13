namespace Veritas.IntellectualProperty;

/// <summary>Represents a validated copyright registration number.</summary>
public readonly struct CopyrightRegistrationNumberValue
{
    /// <summary>The normalized registration number.</summary>
    public string Value { get; }
    public CopyrightRegistrationNumberValue(string value) => Value = value;
    public override string ToString() => Value;
}
