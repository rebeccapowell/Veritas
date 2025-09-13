namespace Veritas.Legal.US;

/// <summary>Represents a validated court case number.</summary>
public readonly struct CourtCaseNumberValue
{
    /// <summary>The normalized court case number.</summary>
    public string Value { get; }
    public CourtCaseNumberValue(string value) => Value = value;
    public override string ToString() => Value;
}
