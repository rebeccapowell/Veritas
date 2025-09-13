using Veritas.Identity;
using Xunit;
using Shouldly;

public class EmailTests
{
    [Theory]
    [InlineData("test@example.com", true)]
    [InlineData("not-an-email", false)]
    public void Validate_Works(string input, bool expected)
    {
        Email.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }
}

