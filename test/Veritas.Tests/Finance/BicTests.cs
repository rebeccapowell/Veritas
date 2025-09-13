using Veritas.Finance;
using Xunit;
using Shouldly;

public class BicTests
{
    [Theory]
    [InlineData("DEUTDEFF", true)]
    [InlineData("DEUT12FF", false)]
    public void Validate_Works(string input, bool expected)
    {
        Bic.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }
}

