using Shouldly;
using Veritas.Tax.TR;
using Xunit;

namespace Veritas.Tests.Tax.TR;

public class TcknTests
{
    [Fact]
    public void GenerateRoundTrip()
    {
        Span<char> dst = stackalloc char[11];
        Tckn.TryGenerate(new GenerationOptions { Seed = 123 }, dst, out var written).ShouldBeTrue();
        var s = new string(dst[..written]);
        Tckn.TryValidate(s, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("10000000146")]
    public void ValidateKnownGood(string input)
    {
        Tckn.TryValidate(input, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("10000000145")]
    [InlineData("abcdefghijk")]
    public void ValidateBad(string input)
    {
        Tckn.TryValidate(input, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeFalse();
    }
}
