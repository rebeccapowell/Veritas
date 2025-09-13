using Veritas.Identity.Luxembourg;
using Veritas;
using Shouldly;
using Xunit;

public class NationalIdTests
{
    [Fact]
    public void Generate_Validates()
    {
        Span<char> dst = stackalloc char[13];
        NationalId.TryGenerate(new GenerationOptions { Seed = 7 }, dst, out var w).ShouldBeTrue();
        var s = new string(dst[..w]);
        NationalId.TryValidate(s, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Invalid_CheckDigits()
    {
        Span<char> dst = stackalloc char[13];
        NationalId.TryGenerate(new GenerationOptions { Seed = 8 }, dst, out var w).ShouldBeTrue();
        dst[11] = dst[11] == '0' ? '1' : '0';
        var s = new string(dst[..w]);
        NationalId.TryValidate(s, out var r).ShouldBeFalse();
        r.IsValid.ShouldBeFalse();
    }
}
