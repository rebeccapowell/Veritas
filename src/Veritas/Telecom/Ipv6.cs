using System;
using System.Net;
using Veritas;

namespace Veritas.Telecom;

/// <summary>Represents a validated IPv6 address.</summary>
public readonly struct Ipv6Value
{
    /// <summary>Gets the normalized IPv6 address string.</summary>
    public string Value { get; }

    /// <summary>Initializes a new instance of the <see cref="Ipv6Value"/> struct.</summary>
    /// <param name="value">The address string.</param>
    public Ipv6Value(string value) => Value = value;
}

/// <summary>Provides validation for IPv6 addresses.</summary>
public static class Ipv6
{
    /// <summary>Attempts to validate the supplied input as an IPv6 address.</summary>
    /// <param name="input">Candidate address to validate.</param>
    /// <param name="result">The validation outcome including the parsed value when valid.</param>
    /// <returns><c>true</c> if validation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<Ipv6Value> result)
    {
        if (IPAddress.TryParse(input, out var addr) && addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
        {
            result = new ValidationResult<Ipv6Value>(true, new Ipv6Value(addr.ToString()), ValidationError.None);
            return true;
        }
        result = new ValidationResult<Ipv6Value>(false, default, ValidationError.Format);
        return false;
    }
}

