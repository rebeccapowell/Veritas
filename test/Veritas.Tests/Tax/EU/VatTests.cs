using Veritas.Tax.EU;
using Xunit;
using Shouldly;

public class VatTests
{
    [Theory]
    [InlineData("EU372000041", true)]
    [InlineData("EU37200004A", false)]
    [InlineData("FR372000041", false)]
    public void Validate(string input, bool expected)
    {
        Vat.TryValidate(input, out var result);
        result.IsValid.ShouldBe(expected);
    }
}
