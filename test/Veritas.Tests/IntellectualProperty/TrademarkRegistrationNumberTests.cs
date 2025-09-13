using Veritas.IntellectualProperty;
using Veritas;
using Shouldly;
using Xunit;

public class TrademarkRegistrationNumberTests
{
    [Fact]
    public void Generate_Validates()
    {
        Span<char> dst = stackalloc char[8];
        TrademarkRegistrationNumber.TryGenerate(new GenerationOptions { Seed = 5 }, dst, out var w).ShouldBeTrue();
        var s = new string(dst[..w]);
        TrademarkRegistrationNumber.TryValidate(s, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Invalid_Format()
    {
        TrademarkRegistrationNumber.TryValidate("12345678", out var r).ShouldBeFalse();
        r.IsValid.ShouldBeFalse();
    }
}
