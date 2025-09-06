using Veritas.Energy.GB;
using Xunit;
using Shouldly;

public class MprnTests
{
    [Theory]
    [InlineData("123456", true)]
    [InlineData("1234567890", true)]
    [InlineData("12345", false)]
    [InlineData("12345678901", false)]
    [InlineData("12345A", false)]
    public void Validate(string input, bool expected)
    {
        Mprn.TryValidate(input, out var result);
        result.IsValid.ShouldBe(expected);
    }
}
