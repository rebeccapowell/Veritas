using System;

namespace Veritas.Energy.DE;

/// <summary>The distinct resource types in BDEW guide 1.2, section 6.</summary>
public enum ResourceIdType
{
    /// <summary>Cluster resource (CR-ID), prefix A.</summary>
    Cluster,
    /// <summary>Control group (SG-ID), prefix B.</summary>
    ControlGroup,
    /// <summary>Controllable resource (SR-ID), prefix C.</summary>
    Controllable,
    /// <summary>Technical resource (TR-ID), prefix D.</summary>
    Technical
}

/// <summary>A structurally and checksum-valid resource identifier; assignment is not verified.</summary>
/// <example>ResourceId.TryValidate("A1137355925", out var result);</example>
public readonly struct ResourceIdValue
{
    /// <summary>Gets the canonical eleven-character identifier.</summary>
    public string Value { get; }
    /// <summary>Gets the resource type identified by its prefix.</summary>
    public ResourceIdType Type { get; }
    internal ResourceIdValue(string value, ResourceIdType type) { Value = value; Type = type; }
    /// <summary>Returns the canonical identifier.</summary>
    public override string ToString() => Value ?? string.Empty;
}

/// <summary>Validates German CR/SG/SR/TR resource IDs and generates synthetic test data.</summary>
/// <remarks>BDEW guide 1.2, sections 6 and 8.2; neither operation establishes allocation.</remarks>
/// <example>ResourceId.TryGenerate(ResourceIdType.Technical, new char[11], out var written);</example>
public static class ResourceId
{
    /// <summary>Checks structure and checksum and classifies the resource type; normalises ASCII case and spaces.</summary>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<ResourceIdValue> result)
    {
        result = new ValidationResult<ResourceIdValue>(false, default, ValidationError.Format);
        if (input.Length > 4096) return false;
        char prefix = '\0';
        foreach (char c in input)
        {
            if (c == ' ') continue;
            prefix = c >= 'a' && c <= 'd' ? (char)(c - 32) : c;
            break;
        }
        ResourceIdType? type = prefix switch
        {
            'A' => ResourceIdType.Cluster,
            'B' => ResourceIdType.ControlGroup,
            'C' => ResourceIdType.Controllable,
            'D' => ResourceIdType.Technical,
            _ => null
        };
        if (!type.HasValue) return false;
        Span<char> buffer = stackalloc char[11];
        if (!BdewAlphanumericId.TryValidate(input, prefix, buffer, out ValidationError error))
        {
            result = new ValidationResult<ResourceIdValue>(false, default, error);
            return false;
        }
        result = new ValidationResult<ResourceIdValue>(true, new ResourceIdValue(new string(buffer), type.Value), ValidationError.None);
        return true;
    }

    /// <summary>Also requires a specific resource type, preserving parsing and checksum errors.</summary>
    public static bool TryValidate(ReadOnlySpan<char> input, ResourceIdType expectedType, out ValidationResult<ResourceIdValue> result)
    {
        if (!TryValidate(input, out result)) return false;
        if (result.Value.Type == expectedType) return true;
        result = new ValidationResult<ResourceIdValue>(false, default, ValidationError.Format);
        return false;
    }

    /// <summary>Generates a synthetic identifier for an explicit resource type; it may coincide with an assigned ID.</summary>
    public static bool TryGenerate(ResourceIdType type, Span<char> destination, out int written)
        => TryGenerate(type, default, destination, out written);

    /// <summary>Generates a synthetic resource ID, deterministically when Seed is supplied.</summary>
    public static bool TryGenerate(ResourceIdType type, in GenerationOptions options, Span<char> destination, out int written)
        => BdewAlphanumericId.TryGenerate(type switch
        {
            ResourceIdType.Cluster => 'A',
            ResourceIdType.ControlGroup => 'B',
            ResourceIdType.Controllable => 'C',
            ResourceIdType.Technical => 'D',
            _ => '\0'
        }, options, destination, out written);
}
