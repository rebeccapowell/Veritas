using Veritas.Identity.Israel;
using Veritas;
using Veritas;
using Xunit;
using Shouldly;

public class TeudatZehutTests
{
    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[9];
        TeudatZehut.TryGenerate(buffer, out var written).ShouldBeTrue();
        TeudatZehut.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void InvalidChecksumFails()
    {
        Span<char> buffer = stackalloc char[9];
        TeudatZehut.TryGenerate(new GenerationOptions { Seed = 6 }, buffer, out var w).ShouldBeTrue();
        buffer[w - 1] = buffer[w - 1] == '0' ? '1' : '0';
        TeudatZehut.TryValidate(buffer[..w], out var result).ShouldBeFalse();
        result.IsValid.ShouldBeFalse();
    }
}
