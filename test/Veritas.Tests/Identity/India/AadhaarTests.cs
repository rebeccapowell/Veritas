using Veritas.Identity.India;
using Veritas;
using Shouldly;
using Xunit;

public class AadhaarTests
{
    [Fact]
    public void Generate_Validates()
    {
        Span<char> dst = stackalloc char[12];
        Aadhaar.TryGenerate(new GenerationOptions { Seed = 123 }, dst, out var written).ShouldBeTrue();
        var s = new string(dst[..written]);
        Aadhaar.TryValidate(s, out var r);
        r.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("123412341235", false)]
    public void Validate_Known(string input, bool expected)
    {
        Aadhaar.TryValidate(input, out var r);
        r.IsValid.ShouldBe(expected);
    }
}
