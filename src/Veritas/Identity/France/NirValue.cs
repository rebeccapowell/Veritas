namespace Veritas.Identity.France;

/// <summary>Represents a validated French NIR (INSEE) number.</summary>
public readonly struct NirValue
{
    /// <summary>Gets the normalized NIR string.</summary>
    public string Value { get; }

    /// <summary>Creates a new <see cref="NirValue"/>.</summary>
    public NirValue(string value) => Value = value;

    public override string ToString() => Value;
}
