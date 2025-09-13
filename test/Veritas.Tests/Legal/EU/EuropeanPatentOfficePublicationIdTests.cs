using Veritas.Legal.EU;
using Veritas;
using Shouldly;
using Xunit;

public class EuropeanPatentOfficePublicationIdTests
{
    [Fact]
    public void Generate_Validates()
    {
        Span<char> dst = stackalloc char[11];
        EuropeanPatentOfficePublicationId.TryGenerate(new GenerationOptions { Seed = 9 }, dst, out var w).ShouldBeTrue();
        var s = new string(dst[..w]);
        EuropeanPatentOfficePublicationId.TryValidate(s, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Invalid_Format()
    {
        EuropeanPatentOfficePublicationId.TryValidate("EP1234567", out var r).ShouldBeFalse();
        r.IsValid.ShouldBeFalse();
    }
}
