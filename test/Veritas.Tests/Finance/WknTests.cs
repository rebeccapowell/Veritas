using Veritas.Finance;
using Xunit;
using Shouldly;

public class WknTests
{
    [Theory]
    [InlineData("M8QAUI", true)]
    [InlineData("M8QAUI1", false)]
    public void Validate_Works(string input, bool expected)
    {
        Wkn.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }
}
