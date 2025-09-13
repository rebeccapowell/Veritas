using Shouldly;
using Veritas.Tax.AT;
using Xunit;

namespace Veritas.Tests.Tax.AT;

public class UidTests
{
    [Fact]
    public void GenerateRoundTrip()
    {
        Span<char> dst = stackalloc char[11];
        Uid.TryGenerate(new GenerationOptions { Seed = 123 }, dst, out var written).ShouldBeTrue();
        var s = new string(dst[..written]);
        Uid.TryValidate(s, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("ATU12345675")]
    public void ValidateKnownGood(string input)
    {
        Uid.TryValidate(input, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("ATU12345674")]
    [InlineData("ATU12345A75")]
    public void ValidateBad(string input)
    {
        Uid.TryValidate(input, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeFalse();
    }
}
