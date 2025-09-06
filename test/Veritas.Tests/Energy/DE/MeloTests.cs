using Veritas.Energy.DE;
using Xunit;
using Shouldly;

public class MeloTests
{
    [Theory]
    [InlineData("123456789012345678901234567890123", true)]
    [InlineData("12345678901234567890123456789012", false)]
    [InlineData("12345678901234567890123456789012A", false)]
    public void Validate(string input, bool expected)
    {
        Melo.TryValidate(input, out var result);
        result.IsValid.ShouldBe(expected);
    }
}
