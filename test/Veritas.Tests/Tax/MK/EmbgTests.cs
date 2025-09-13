using Shouldly;
using Veritas.Tax.MK;
using Xunit;

namespace Veritas.Tests.Tax.MK;

public class EmbgTests
{
    [Fact]
    public void GenerateRoundTrip()
    {
        Span<char> dst = stackalloc char[13];
        Embg.TryGenerate(new GenerationOptions { Seed = 123 }, dst, out var written).ShouldBeTrue();
        var s = new string(dst[..written]);
        Embg.TryValidate(s, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("0101006500006")]
    public void ValidateKnownGood(string input)
    {
        Embg.TryValidate(input, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("0101006500007")]
    [InlineData("abcdefghijklm")]
    public void ValidateBad(string input)
    {
        Embg.TryValidate(input, out var r).ShouldBeFalse();
        r.IsValid.ShouldBeFalse();
    }
}
