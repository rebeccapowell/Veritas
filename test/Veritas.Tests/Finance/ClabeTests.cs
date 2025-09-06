using Veritas.Finance;
using Xunit;
using Shouldly;

public class ClabeTests
{
    [Theory]
    [InlineData("349313962323709749", true)]
    [InlineData("349313962323709740", false)]
    [InlineData("34931396232370974", false)]
    public void Validate(string input, bool expected)
    {
        Clabe.TryValidate(input, out var result);
        result.IsValid.ShouldBe(expected);
    }
}
