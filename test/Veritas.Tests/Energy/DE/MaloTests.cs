using Veritas.Energy.DE;
using Xunit;
using Shouldly;

public class MaloTests
{
    [Theory]
    [InlineData("12345678901", true)]
    [InlineData("1234567890", false)]
    [InlineData("1234567890A", false)]
    public void Validate(string input, bool expected)
    {
        Malo.TryValidate(input, out var result);
        result.IsValid.ShouldBe(expected);
    }
}
