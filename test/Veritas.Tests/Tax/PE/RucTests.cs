using Shouldly;
using Veritas.Tax.PE;
using Xunit;

namespace Veritas.Tests.Tax.PE;

public class RucTests
{
    [Fact]
    public void GenerateRoundTrip()
    {
        Span<char> dst = stackalloc char[11];
        Ruc.TryGenerate(new GenerationOptions { Seed = 123 }, dst, out var written).ShouldBeTrue();
        var s = new string(dst[..written]);
        Ruc.TryValidate(s, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("20100070970")]
    public void ValidateKnownGood(string input)
    {
        Ruc.TryValidate(input, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("20100070971")]
    [InlineData("abcdefghijk")]
    public void ValidateBad(string input)
    {
        Ruc.TryValidate(input, out var r).ShouldBeFalse();
        r.IsValid.ShouldBeFalse();
    }
}
