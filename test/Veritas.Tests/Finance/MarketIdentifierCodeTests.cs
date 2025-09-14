using Veritas.Finance;
using Xunit;
using Shouldly;

public class MarketIdentifierCodeTests
{
    [Theory]
    [InlineData("XNAS", true)]
    [InlineData("abcd", true)]
    [InlineData("ABC", false)]
    [InlineData("ABCDE", false)]
    [InlineData("AB1$", false)]
    public void Validate(string input, bool expected)
    {
        MarketIdentifierCode.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[4];
        MarketIdentifierCode.TryGenerate(buffer, out var written).ShouldBeTrue();
        MarketIdentifierCode.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

