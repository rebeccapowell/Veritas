using Veritas.IntellectualProperty;
using Veritas;
using Shouldly;
using Xunit;

public class IswcTests
{
    [Fact]
    public void Generate_Validates()
    {
        Span<char> dst = stackalloc char[11];
        Iswc.TryGenerate(new GenerationOptions { Seed = 1 }, dst, out var w).ShouldBeTrue();
        var s = new string(dst[..w]);
        Iswc.TryValidate(s, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Invalid_Check()
    {
        Span<char> dst = stackalloc char[11];
        Iswc.TryGenerate(new GenerationOptions { Seed = 2 }, dst, out var w).ShouldBeTrue();
        dst[10] = dst[10] == '0' ? '1' : '0';
        var s = new string(dst[..w]);
        Iswc.TryValidate(s, out var r).ShouldBeFalse();
        r.IsValid.ShouldBeFalse();
    }
}
