using Veritas.Energy;
using Xunit;
using Shouldly;

public class EicTests
{
    [Theory]
    [InlineData("XAT000001234567Y", true)]
    [InlineData("XAT000001234567Z", false)]
    [InlineData("XAT000001234567", false)]
    public void Validate_Works(string input, bool expected)
    {
        Eic.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }
}

