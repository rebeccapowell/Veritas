using Shouldly;
using Veritas.Tax.SI;
using Xunit;

namespace Veritas.Tests.Tax.SI;

public class EmsoTests
{
    [Fact]
    public void GenerateRoundTrip()
    {
        Span<char> dst = stackalloc char[13];
        Emso.TryGenerate(new GenerationOptions { Seed = 123 }, dst, out var written).ShouldBeTrue();
        var s = new string(dst[..written]);
        Emso.TryValidate(s, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("0101006500006")]
    public void ValidateKnownGood(string input)
    {
        Emso.TryValidate(input, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("0101006500007")]
    [InlineData("abcdefghijklm")]
    public void ValidateBad(string input)
    {
        Emso.TryValidate(input, out var r).ShouldBeFalse();
        r.IsValid.ShouldBeFalse();
    }
}
