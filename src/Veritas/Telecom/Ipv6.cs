using System;
using System.Net;
using Veritas;

namespace Veritas.Telecom;

public readonly struct Ipv6Value { public string Value { get; } public Ipv6Value(string v) => Value = v; }

public static class Ipv6
{
    public static bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<Ipv6Value> result)
    {
        if (IPAddress.TryParse(input, out var addr) && addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
        {
            result = new ValidationResult<Ipv6Value>(true, new Ipv6Value(addr.ToString()), ValidationError.None);
            return true;
        }
        result = new ValidationResult<Ipv6Value>(false, default, ValidationError.Format);
        return true;
    }
}
