using Shouldly;
using Veritas.Tax.CH;
using Xunit;

namespace Veritas.Tests.Tax.CH;

public class UidTests
{
    [Fact]
    public void GenerateRoundTrip()
    {
        Span<char> dst = stackalloc char[12];
        Uid.TryGenerate(new GenerationOptions { Seed = 123 }, dst, out var written).ShouldBeTrue();
        var s = new string(dst[..written]);
        Uid.TryValidate(s, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("CHE100155709")]
    [InlineData("CHE-100.155.709 MWST")]
    public void ValidateKnownGood(string input)
    {
        Uid.TryValidate(input, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("CHE100155708")]
    [InlineData("CHE100155709XYZ")]
    public void ValidateBad(string input)
    {
        Uid.TryValidate(input, out var r).ShouldBeFalse();
        r.IsValid.ShouldBeFalse();
    }
}
