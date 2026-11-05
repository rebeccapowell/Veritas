using Veritas.Energy;
using Xunit;
using Shouldly;

public class EicTests
{
    [Theory]
    [InlineData("11Y123456789012T", true)]
    [InlineData("11Y123456789012Z", false)]
    [InlineData("XAT000001234567", false)]
    public void Validate_Works(string input, bool expected)
    {
        Eic.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }
}

