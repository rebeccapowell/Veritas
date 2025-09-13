using Shouldly;
using Veritas.Tax.BG;
using Xunit;

namespace Veritas.Tests.Tax.BG;

public class EgnTests
{
    [Fact]
    public void GenerateRoundTrip()
    {
        Span<char> dst = stackalloc char[10];
        Egn.TryGenerate(new GenerationOptions { Seed = 123 }, dst, out var written).ShouldBeTrue();
        var s = new string(dst[..written]);
        Egn.TryValidate(s, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("7523169263")]
    public void ValidateKnownGood(string input)
    {
        Egn.TryValidate(input, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("7523169264")]
    [InlineData("abcdefghij")]
    public void ValidateBad(string input)
    {
        Egn.TryValidate(input, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeFalse();
    }
}
