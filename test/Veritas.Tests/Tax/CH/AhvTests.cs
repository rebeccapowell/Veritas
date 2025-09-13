using Shouldly;
using Veritas.Tax.CH;
using Xunit;

namespace Veritas.Tests.Tax.CH;

public class AhvTests
{
    [Fact]
    public void GenerateRoundTrip()
    {
        Span<char> dst = stackalloc char[13];
        Ahv.TryGenerate(new GenerationOptions { Seed = 123 }, dst, out var written).ShouldBeTrue();
        var s = new string(dst[..written]);
        Ahv.TryValidate(s, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("7569217076985")]
    public void ValidateKnownGood(string input)
    {
        Ahv.TryValidate(input, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("7569217076984")]
    [InlineData("756.9217.0769.85a")]
    public void ValidateBad(string input)
    {
        Ahv.TryValidate(input, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeFalse();
    }
}
