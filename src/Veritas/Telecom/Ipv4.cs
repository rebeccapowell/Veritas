using System;
using System.Net;
using Veritas;

namespace Veritas.Telecom;

public readonly struct Ipv4Value { public string Value { get; } public Ipv4Value(string v) => Value = v; }

public static class Ipv4
{
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
