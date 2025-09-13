using Veritas.Identity.France;
using Veritas;
using Shouldly;
using Xunit;

public class NirTests
{
    [Fact]
    public void Generate_Validates()
    {
        Span<char> dst = stackalloc char[15];
        Nir.TryGenerate(new GenerationOptions { Seed = 42 }, dst, out var written).ShouldBeTrue();
        var s = new string(dst[..written]);
        Nir.TryValidate(s, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("1 00 01 2A 001 001 74", true)]
    [InlineData("123456789012311", true)]
    [InlineData("100012A00100175", false)]
    [InlineData("100012A0010017X", false)]
    [InlineData("100012A001001", false)]
    public void Validate_Known(string input, bool expected)
    {
        Nir.TryValidate(input, out var r).ShouldBe(expected);
        r.IsValid.ShouldBe(expected);
    }
}
