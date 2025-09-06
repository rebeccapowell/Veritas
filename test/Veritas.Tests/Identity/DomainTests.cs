using Veritas.Identity;
using Xunit;
using Shouldly;

public class DomainTests
{
    [Theory]
    [InlineData("example.com", true)]
    [InlineData("-bad.com", false)]
    public void Validate(string input, bool expected)
    {
        Domain.TryValidate(input, out var r);
        r.IsValid.ShouldBe(expected);
    }
}
