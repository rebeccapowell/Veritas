using Veritas.IntellectualProperty;
using Veritas;
using Shouldly;
using Xunit;

public class PatentPublicationNumberTests
{
    [Fact]
    public void Generate_Validates()
    {
        Span<char> dst = stackalloc char[11];
        PatentPublicationNumber.TryGenerate(new GenerationOptions { Seed = 4 }, dst, out var w).ShouldBeTrue();
        var s = new string(dst[..w]);
        PatentPublicationNumber.TryValidate(s, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Invalid_Format()
    {
        PatentPublicationNumber.TryValidate("USABC12345", out var r).ShouldBeFalse();
        r.IsValid.ShouldBeFalse();
    }
}
