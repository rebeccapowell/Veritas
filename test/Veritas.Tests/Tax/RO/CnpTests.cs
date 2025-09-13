using Shouldly;
using Veritas.Tax.RO;
using Xunit;

namespace Veritas.Tests.Tax.RO;

public class CnpTests
{
    [Fact]
    public void GenerateRoundTrip()
    {
        Span<char> dst = stackalloc char[13];
        Cnp.TryGenerate(new GenerationOptions { Seed = 123 }, dst, out var written).ShouldBeTrue();
        var s = new string(dst[..written]);
        Cnp.TryValidate(s, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
        r.Value.Value.ShouldBe(s);
    }

    [Theory]
    [InlineData("1960523460016")]
    public void ValidateKnownGood(string input)
    {
        Cnp.TryValidate(input, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("1960523460017")]
    [InlineData("abcdefghijklm")]
    public void ValidateBad(string input)
    {
        Cnp.TryValidate(input, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeFalse();
    }
}
