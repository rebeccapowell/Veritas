using Shouldly;
using Veritas.Tax.DK;
using Xunit;

namespace Veritas.Tests.Tax.DK;

public class CprTests
{
    [Fact]
    public void GenerateRoundTrip()
    {
        Span<char> dst = stackalloc char[10];
        Cpr.TryGenerate(new GenerationOptions { Seed = 123 }, dst, out var written).ShouldBeTrue();
        var s = new string(dst[..written]);
        Cpr.TryValidate(s, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("120174-3399")]
    public void ValidateKnownGood(string input)
    {
        Cpr.TryValidate(input, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("0000000000")]
    [InlineData("abcdefghij")]
    public void ValidateBad(string input)
    {
        Cpr.TryValidate(input, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeFalse();
    }
}
