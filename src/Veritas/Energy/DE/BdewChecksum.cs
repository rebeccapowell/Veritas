using System;

namespace Veritas.Energy.DE;

/// <summary>BDEW application guide 1.2, section 8.1: Lok-/Waggon numeric check digit.</summary>
internal static class BdewChecksum
{
    internal static int ComputeNumeric(ReadOnlySpan<char> body)
    {
        int sum = 0;
        for (int i = 0; i < body.Length; i++) sum += (body[i] - '0') * (i % 2 == 0 ? 1 : 2);
        return (10 - sum % 10) % 10;
    }
}
