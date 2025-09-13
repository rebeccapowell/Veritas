using Veritas.Tax.NZ;
using Xunit;
using Shouldly;

public class IrdTests
{
    [Theory]
    [InlineData("49091850", true)]
    [InlineData("49091851", false)]
    public void Validate_Works(string input, bool expected)
    {
        Ird.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }
}
