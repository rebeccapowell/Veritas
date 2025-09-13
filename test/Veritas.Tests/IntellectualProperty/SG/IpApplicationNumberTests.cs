using Veritas.IntellectualProperty.SG;
using Veritas;
using Shouldly;
using Xunit;

public class IpApplicationNumberTests
{
    [Fact]
    public void Generate_Validates()
    {
        Span<char> dst = stackalloc char[12];
        IpApplicationNumber.TryGenerate(new GenerationOptions { Seed = 99 }, dst, out var w).ShouldBeTrue();
        var s = new string(dst[..w]);
        IpApplicationNumber.TryValidate(s, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Invalid_Check()
    {
        Span<char> dst = stackalloc char[12];
        IpApplicationNumber.TryGenerate(new GenerationOptions { Seed = 100 }, dst, out var w).ShouldBeTrue();
        dst[11] = dst[11] == '0' ? '1' : '0';
        var s = new string(dst[..w]);
        IpApplicationNumber.TryValidate(s, out var r).ShouldBeFalse();
        r.IsValid.ShouldBeFalse();
    }
}
