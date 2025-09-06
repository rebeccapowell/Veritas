using Veritas.Finance;
using Xunit;
using Shouldly;

public class AbaRoutingTests
{
    [Theory]
    [InlineData("111000025", true)]
    [InlineData("111000026", false)]
    public void Validate_Works(string input, bool expected)
    {
        AbaRouting.TryValidate(input, out var result);
        result.IsValid.ShouldBe(expected);
    }
}

