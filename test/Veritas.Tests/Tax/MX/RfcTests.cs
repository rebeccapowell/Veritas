using Veritas.Tax.MX;
using Veritas;
using Xunit;
using Shouldly;

public class RfcTests
{
    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[13];
        Rfc.TryGenerate(buffer, out var written).ShouldBeTrue();
        Rfc.TryValidate(buffer[..written], out var result);
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void InvalidChecksumFails()
    {
        Span<char> buffer = stackalloc char[13];
        Rfc.TryGenerate(new GenerationOptions { Seed = 2 }, buffer, out var w).ShouldBeTrue();
        buffer[w - 1] = buffer[w - 1] == '0' ? '1' : '0';
        Rfc.TryValidate(buffer[..w], out var result);
        result.IsValid.ShouldBeFalse();
    }
}
