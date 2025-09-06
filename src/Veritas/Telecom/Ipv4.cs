using System;
using System.Net;
using Veritas;

namespace Veritas.Telecom;

/// <summary>Represents a validated IPv4 address.</summary>
public readonly struct Ipv4Value
{
    /// <summary>Gets the normalized IPv4 address string.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="Ipv4Value"/> struct.</summary>
    /// <param name="value">The address string.</param>
    public Ipv4Value(string value) => Value = value;
}

/// <summary>Provides validation for IPv4 addresses.</summary>
public static class Ipv4
{
    /// <summary>Attempts to validate the supplied input as an IPv4 address.</summary>
    /// <param name="input">Candidate address to validate.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation executed; the <see cref="ValidationResult{T}.IsValid"/> property indicates success.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<Ipv4Value> result)
    {
        if (IPAddress.TryParse(input, out var addr) && addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
        {
            result = new ValidationResult<Ipv4Value>(true, new Ipv4Value(addr.ToString()), ValidationError.None);
            return true;
        }
        result = new ValidationResult<Ipv4Value>(false, default, ValidationError.Format);
        return true;
    }
}

