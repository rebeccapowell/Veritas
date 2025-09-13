using Shouldly;
using Veritas.Tax.IS;
using Xunit;

namespace Veritas.Tests.Tax.IS;

public class KennitalaTests
{
    [Fact]
    public void GenerateRoundTrip()
    {
        Span<char> dst = stackalloc char[10];
        Kennitala.TryGenerate(new GenerationOptions { Seed = 123 }, dst, out var written).ShouldBeTrue();
        var s = new string(dst[..written]);
        Kennitala.TryValidate(s, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("1201743399")]
    public void ValidateKnownGood(string input)
    {
        Kennitala.TryValidate(input, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("1201743398")]
    [InlineData("abcdefghij")]
    public void ValidateBad(string input)
    {
        Kennitala.TryValidate(input, out var r).ShouldBeTrue();
        r.IsValid.ShouldBeFalse();
    }
}
