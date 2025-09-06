using Veritas.Finance;
using Xunit;
using Shouldly;

public class MicTests
{
    [Theory]
    [InlineData("XNAS", true)]
    [InlineData("ABC", false)]
    public void Validate_Works(string input, bool expected)
    {
        Mic.TryValidate(input, out var result);
        result.IsValid.ShouldBe(expected);
    }
}
