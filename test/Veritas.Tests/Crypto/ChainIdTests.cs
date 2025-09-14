using Veritas.Crypto;
using Xunit;
using Shouldly;

public class ChainIdTests
{
    [Theory]
    [InlineData("1", true)]
    [InlineData("137", true)]
    [InlineData("0", false)]
    [InlineData("abc", false)]
    public void Validate(string input, bool expected)
    {
        ChainId.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[10];
        ChainId.TryGenerate(buffer, out var written).ShouldBeTrue();
        ChainId.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}
