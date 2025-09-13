using Veritas.Energy.FR;
using Xunit;
using Shouldly;

public class PrmTests
{
    [Theory]
    [InlineData("12345678901234", true)]
    [InlineData("1234567890123", false)]
    [InlineData("1234567890123A", false)]
    public void Validate(string input, bool expected)
    {
        Prm.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }
}
