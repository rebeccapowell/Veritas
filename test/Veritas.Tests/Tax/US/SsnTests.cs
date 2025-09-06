using Veritas.Tax.US;
using Xunit;
using Shouldly;

public class SsnTests
{
    [Theory]
    [InlineData("123-45-6789", true)]
    [InlineData("000-00-0000", false)]
    public void Validate(string input, bool expected)
    {
        Ssn.TryValidate(input, out var r);
        r.IsValid.ShouldBe(expected);
    }
}
