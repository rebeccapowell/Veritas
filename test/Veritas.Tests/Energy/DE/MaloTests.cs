using Veritas.Energy.DE;
using Xunit;
using Shouldly;

public class MaloTests
{
    [Theory]
    [InlineData("49637777476", true)]
    [InlineData("49637777475", false)]
    [InlineData("1234567890A", false)]
    public void Validate(string input, bool expected)
    {
        Malo.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }
}
