namespace Veritas;

/// <summary>Represents error categories when validating an identifier.</summary>
public enum ValidationError
{
    None,
    Length,
    Charset,
    Checksum,
    CountryRule,
    Format,
    Range,
    ReservedPrefix
}
