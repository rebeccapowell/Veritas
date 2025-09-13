using Shouldly;
using Veritas.Tax.LV;
using Xunit;

namespace Veritas.Tests.Tax.LV;

public class PersonasKodsTests
{
    [Fact]
    public void GenerateRoundTrip()
    {
        Span<char> dst = stackalloc char[11];
        PersonasKods.TryGenerate(new GenerationOptions { Seed = 123 }, dst, out var written).ShouldBeTrue();
        var s = new string(dst[..written]);
        PersonasKods.TryValidate(s, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("120174-33913")]
    public void ValidateKnownGood(string input)
    {
        PersonasKods.TryValidate(input, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("12017433918")]
    [InlineData("abcdefghijk")]
    public void ValidateBad(string input)
    {
        PersonasKods.TryValidate(input, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeFalse();
    }
}
