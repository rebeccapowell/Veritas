using Veritas.Finance;
using Xunit;
using Shouldly;

public class IsinTests
{
    [Theory]
    [InlineData("US0378331005", true)]
    [InlineData("US0378331006", false)]
    public void Validate_Works(string input, bool expected)
    {
        Isin.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }
}

