using Veritas.Tax.SG;
using Veritas;
using Xunit;
using Shouldly;

public class UenTests
{
    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[10];
        Uen.TryGenerate(buffer, out var written).ShouldBeTrue();
        Uen.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void InvalidChecksumFails()
    {
        Span<char> buffer = stackalloc char[10];
        Uen.TryGenerate(new GenerationOptions { Seed = 4 }, buffer, out var w).ShouldBeTrue();
        buffer[w - 1] = buffer[w - 1] == '0' ? '1' : '0';
        Uen.TryValidate(buffer[..w], out var result).ShouldBeFalse();
        result.IsValid.ShouldBeFalse();
    }
}
