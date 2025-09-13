using Veritas.Legal.EU;
using Veritas;
using Shouldly;
using Xunit;

public class EcliTests
{
    [Fact]
    public void Generate_Validates()
    {
        Span<char> dst = stackalloc char[30];
        Ecli.TryGenerate(new GenerationOptions { Seed = 7 }, dst, out var w).ShouldBeTrue();
        var s = new string(dst[..w]);
        Ecli.TryValidate(s, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Invalid_Format()
    {
        Ecli.TryValidate("ECLI-NO-CODE", out var r).ShouldBeFalse();
        r.IsValid.ShouldBeFalse();
    }
}
