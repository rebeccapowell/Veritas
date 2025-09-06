using Veritas.Identity;
using Xunit;
using Shouldly;

public class EthereumTests
{
    [Theory]
    [InlineData("0xde709f2102306220921060314715629080e2fb77", true)]
    [InlineData("0x123", false)]
    [InlineData("zz709f2102306220921060314715629080e2fb77", false)]
    public void Validate(string input, bool expected)
    {
        Ethereum.TryValidate(input, out var r);
        r.IsValid.ShouldBe(expected);
    }
}
