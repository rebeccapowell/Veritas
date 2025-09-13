using Veritas.Telecom;
using Xunit;
using Shouldly;

public class OuiTests
{
    [Theory]
    [InlineData("00A0C9", true)]
    [InlineData("00-A0-C9", true)]
    [InlineData("00A0C", false)]
    [InlineData("00A0CZ", false)]
    public void Validate(string input, bool expected)
    {
        Oui.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }
}
