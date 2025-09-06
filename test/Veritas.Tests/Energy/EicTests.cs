using Veritas.Energy;
using Xunit;
using Shouldly;

public class EicTests
{
    [Theory]
    [InlineData("XAT0000012345678", true)]
    [InlineData("XAT000001234567", false)]
    public void Validate_Works(string input, bool expected)
    {
        Eic.TryValidate(input, out var result);
        result.IsValid.ShouldBe(expected);
    }
}

