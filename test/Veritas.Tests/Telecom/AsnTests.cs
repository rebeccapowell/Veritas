using Veritas.Telecom;
using Xunit;
using Shouldly;

public class AsnTests
{
    [Theory]
    [InlineData("64512", true)]
    [InlineData("4294967295", true)]
    [InlineData("4294967296", false)]
    [InlineData("-1", false)]
    [InlineData("abc", false)]
    public void Validate(string input, bool expected)
    {
        Asn.TryValidate(input, out var result);
        result.IsValid.ShouldBe(expected);
    }
}
