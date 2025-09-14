using Veritas.Crypto;
using Xunit;
using Shouldly;

public class EthereumTransactionHashTests
{
    [Theory]
    [InlineData("0x5e843feeb5642ae4e2033ae0f665b9230e650ba9ee3b71c008651675c1bc5db4", true)]
    [InlineData("5e843feeb5642ae4e2033ae0f665b9230e650ba9ee3b71c008651675c1bc5db4", true)]
    [InlineData("0x123", false)]
    [InlineData("zz843feeb5642ae4e2033ae0f665b9230e650ba9ee3b71c008651675c1bc5db4", false)]
    public void Validate(string input, bool expected)
    {
        EthereumTransactionHash.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[66];
        EthereumTransactionHash.TryGenerate(buffer, out var written).ShouldBeTrue();
        EthereumTransactionHash.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}
