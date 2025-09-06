using Veritas.Tax.SE;
using Xunit;
using Shouldly;

public class OrgNrTests
{
    [Theory]
    [InlineData("556016-0680", true)]
    [InlineData("5560160681", false)]
    public void Validate(string input, bool expected)
    {
        OrgNr.TryValidate(input, out var result);
        result.IsValid.ShouldBe(expected);
    }
}
