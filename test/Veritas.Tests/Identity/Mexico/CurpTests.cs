using Veritas.Identity.Mexico;
using Veritas;
using Xunit;
using Shouldly;

public class CurpTests
{
    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[18];
        Curp.TryGenerate(buffer, out var written).ShouldBeTrue();
        Curp.TryValidate(buffer[..written], out var result);
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void InvalidChecksumFails()
    {
        Span<char> buffer = stackalloc char[18];
        Curp.TryGenerate(new GenerationOptions { Seed = 3 }, buffer, out var w).ShouldBeTrue();
        buffer[w - 1] = buffer[w - 1] == '0' ? '1' : '0';
        Curp.TryValidate(buffer[..w], out var result);
        result.IsValid.ShouldBeFalse();
    }
}
