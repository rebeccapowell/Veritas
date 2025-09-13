using Shouldly;
using Veritas.Tax.EE;
using Xunit;

namespace Veritas.Tests.Tax.EE;

public class IsikukoodTests
{
    [Fact]
    public void GenerateRoundTrip()
    {
        Span<char> dst = stackalloc char[11];
        Isikukood.TryGenerate(new GenerationOptions { Seed = 123 }, dst, out var written).ShouldBeTrue();
        var s = new string(dst[..written]);
        Isikukood.TryValidate(s, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("39912319997")]
    public void ValidateKnownGood(string input)
    {
        Isikukood.TryValidate(input, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("39912319995")]
    [InlineData("abcdefghijk")]
    public void ValidateBad(string input)
    {
        Isikukood.TryValidate(input, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeFalse();
    }
}
