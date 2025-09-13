using Veritas.Legal.US;
using Veritas;
using Shouldly;
using Xunit;

public class CourtCaseNumberTests
{
    [Fact]
    public void Generate_Validates()
    {
        Span<char> dst = stackalloc char[11];
        CourtCaseNumber.TryGenerate(new GenerationOptions { Seed = 8 }, dst, out var w).ShouldBeTrue();
        var s = new string(dst[..w]);
        CourtCaseNumber.TryValidate(s, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Invalid_Format()
    {
        CourtCaseNumber.TryValidate("2023-12345", out var r).ShouldBeFalse();
        r.IsValid.ShouldBeFalse();
    }
}
