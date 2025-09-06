using Veritas.Tax.PL;
using Xunit;
using Shouldly;

public class RegonTests
{
    [Theory]
    [InlineData("122923026", true)]
    [InlineData("122923027", false)]
    [InlineData("12345678901234", true)]
    [InlineData("12345678901235", false)]
    public void Validate(string input, bool expected)
    {
        Regon.TryValidate(input, out var result);
        result.IsValid.ShouldBe(expected);
    }
}
