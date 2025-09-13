using Veritas.Tax.IT;
using Veritas;
using Xunit;
using Shouldly;

public class CodiceFiscaleTests
{
    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[16];
        CodiceFiscale.TryGenerate(buffer, out var written).ShouldBeTrue();
        CodiceFiscale.TryValidate(buffer[..written], out var result);
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void InvalidChecksumFails()
    {
        Span<char> buffer = stackalloc char[16];
        CodiceFiscale.TryGenerate(new GenerationOptions { Seed = 1 }, buffer, out var w).ShouldBeTrue();
        buffer[w - 1] = buffer[w - 1] == 'A' ? 'B' : 'A';
        CodiceFiscale.TryValidate(buffer[..w], out var result);
        result.IsValid.ShouldBeFalse();
    }
}
