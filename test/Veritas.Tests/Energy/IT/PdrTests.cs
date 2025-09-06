using Veritas.Energy.IT;
using Xunit;
using Shouldly;

public class PdrTests
{
    [Theory]
    [InlineData("12345678901234", true)]
    [InlineData("1234567890123", false)]
    public void Validate_Works(string input, bool expected)
    {
        Pdr.TryValidate(input, out var result);
        result.IsValid.ShouldBe(expected);
    }
}
