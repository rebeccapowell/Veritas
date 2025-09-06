namespace Veritas;

/// <summary>Options controlling identifier generation.</summary>
public readonly struct GenerationOptions
{
    /// <summary>Number of identifiers to generate.</summary>
    public int Count { get; init; }

    /// <summary>Optional seed for deterministic generation.</summary>
    public int? Seed { get; init; }
}
