namespace Veritas.Finance.BE;

/// <summary>Represents a validated Belgian structured communication reference.</summary>
public readonly struct OgmValue
{
    /// <summary>Gets the normalized reference string.</summary>
    public string Value { get; }

    /// <summary>Initializes a new <see cref="OgmValue"/>.</summary>
    public OgmValue(string value) => Value = value;

    public override string ToString() => Value;
}
