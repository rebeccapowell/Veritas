using Shouldly;
using Veritas.Tax.CO;
using Xunit;

namespace Veritas.Tests.Tax.CO;

public class NitTests
{
    [Fact]
    public void GenerateRoundTrip()
    {
        Span<char> dst = stackalloc char[10];
        Nit.TryGenerate(new GenerationOptions { Seed = 123 }, dst, out var written).ShouldBeTrue();
        var s = new string(dst[..written]);
        Nit.TryValidate(s, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("8001972681")]
    public void ValidateKnownGood(string input)
    {
        Nit.TryValidate(input, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("8001972680")]
    [InlineData("abcdefghij")]
    public void ValidateBad(string input)
    {
        Nit.TryValidate(input, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeFalse();
    }
}
