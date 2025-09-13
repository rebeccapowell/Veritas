using Veritas.IntellectualProperty;
using Veritas;
using Shouldly;
using Xunit;

public class PatentApplicationNumberTests
{
    [Fact]
    public void Generate_Validates()
    {
        Span<char> dst = stackalloc char[10];
        PatentApplicationNumber.TryGenerate(new GenerationOptions { Seed = 3 }, dst, out var w).ShouldBeTrue();
        var s = new string(dst[..w]);
        PatentApplicationNumber.TryValidate(s, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Invalid_Format()
    {
        PatentApplicationNumber.TryValidate("1A00000000", out var r).ShouldBeFalse();
        r.IsValid.ShouldBeFalse();
    }
}
