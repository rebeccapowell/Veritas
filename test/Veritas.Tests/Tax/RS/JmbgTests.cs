using Shouldly;
using Veritas.Tax.RS;
using Xunit;

namespace Veritas.Tests.Tax.RS;

public class JmbgTests
{
    [Fact]
    public void GenerateRoundTrip()
    {
        Span<char> dst = stackalloc char[13];
        Jmbg.TryGenerate(new GenerationOptions { Seed = 123 }, dst, out var written).ShouldBeTrue();
        var s = new string(dst[..written]);
        Jmbg.TryValidate(s, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("0101006500006")]
    public void ValidateKnownGood(string input)
    {
        Jmbg.TryValidate(input, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("0101006500007")]
    [InlineData("abcdefghijklm")]
    public void ValidateBad(string input)
    {
        Jmbg.TryValidate(input, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeFalse();
    }
}
