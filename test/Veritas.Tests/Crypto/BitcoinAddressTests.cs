using Veritas.Crypto;
using Xunit;
using Shouldly;

public class BitcoinAddressTests
{
    [Theory]
    [InlineData("1BoatSLRHtKNngkdXEeobR76b53LETtpyT", true)]
    [InlineData("3J98t1WpEZ73CNmQviecrnyiWrnqRhWNLy", true)]
    [InlineData("0OIl", false)]
    public void Validate(string input, bool expected)
    {
        BitcoinAddress.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[50];
        BitcoinAddress.TryGenerate(buffer, out var written).ShouldBeTrue();
        BitcoinAddress.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}
