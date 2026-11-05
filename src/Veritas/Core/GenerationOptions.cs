using System;

namespace Veritas;

/// <summary>Options controlling identifier generation.</summary>
public readonly struct GenerationOptions
{
    /// <summary>Number of identifiers to generate.</summary>
    public int Count { get; init; }

    /// <summary>Optional timestamp for ULID/KSUID generation; seeded fixtures otherwise use their scheme epoch.</summary>
    public DateTimeOffset? Timestamp { get; init; }

    /// <summary>Optional seed for deterministic synthetic generation, including a fixed epoch for ULID/KSUID.</summary>
    public int? Seed { get; init; }
}
