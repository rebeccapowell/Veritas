using Veritas.Finance.BE;
using Veritas;
using Shouldly;
using Xunit;

public class OgmTests
{
    [Fact]
    public void Generate_Validates()
    {
        Span<char> dst = stackalloc char[12];
        Ogm.TryGenerate(new GenerationOptions { Seed = 123 }, dst, out var written).ShouldBeTrue();
        var s = new string(dst[..written]);
        Ogm.TryValidate(s, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("+++123/4567/89002+++", true)]
    [InlineData("123456789002", true)]
    [InlineData("123456789001", false)]
    [InlineData("1234567890", false)]
    [InlineData("123/4567/8900X", false)]
    public void Validate_Known(string input, bool expected)
    {
        Ogm.TryValidate(input, out var r).ShouldBe(expected);
        r.IsValid.ShouldBe(expected);
    }
}
