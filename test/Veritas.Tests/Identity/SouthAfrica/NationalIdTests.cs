using Veritas.Identity.SouthAfrica;
using Veritas;
using Xunit;
using Shouldly;

public class SouthAfricaNationalIdTests
{
    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[13];
        NationalId.TryGenerate(buffer, out var written).ShouldBeTrue();
        NationalId.TryValidate(buffer[..written], out var result);
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void InvalidChecksumFails()
    {
        Span<char> buffer = stackalloc char[13];
        NationalId.TryGenerate(new GenerationOptions { Seed = 5 }, buffer, out var w).ShouldBeTrue();
        buffer[w - 1] = buffer[w - 1] == '0' ? '1' : '0';
        NationalId.TryValidate(buffer[..w], out var result);
        result.IsValid.ShouldBeFalse();
    }
}
