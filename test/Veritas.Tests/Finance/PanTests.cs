using Veritas.Finance;
using Xunit;
using Shouldly;

public class PanTests
{
    [Theory]
    [InlineData("4111 1111 1111 1111", true)]
    [InlineData("4111 1111 1111 1112", false)]
    public void Validate_Works(string input, bool expected)
    {
        Pan.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }
}

