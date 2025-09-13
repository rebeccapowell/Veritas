using Veritas.Finance;
using Xunit;
using Shouldly;

public class IbanTests
{
    [Theory]
    [InlineData("FR14 2004 1010 0505 0001 3M02 606", true)]
    [InlineData("FR14 2004 1010 0505 0001 3M02 607", false)]
    public void Validate_Works(string input, bool expected)
    {
        Iban.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }
}

