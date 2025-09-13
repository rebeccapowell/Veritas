using Shouldly;
using Veritas.Tax.LT;
using Xunit;

namespace Veritas.Tests.Tax.LT;

public class AsmensKodasTests
{
    [Fact]
    public void GenerateRoundTrip()
    {
        Span<char> dst = stackalloc char[11];
        AsmensKodas.TryGenerate(new GenerationOptions { Seed = 123 }, dst, out var written).ShouldBeTrue();
        var s = new string(dst[..written]);
        AsmensKodas.TryValidate(s, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("39912319997")]
    public void ValidateKnownGood(string input)
    {
        AsmensKodas.TryValidate(input, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("39912319996")]
    [InlineData("abcdefghijk")]
    public void ValidateBad(string input)
    {
        AsmensKodas.TryValidate(input, out var r).ShouldBeFalse();
        r.IsValid.ShouldBeFalse();
    }
}
