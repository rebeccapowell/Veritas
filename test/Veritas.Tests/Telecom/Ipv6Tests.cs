using Veritas.Telecom;
using Xunit;
using Shouldly;

public class Ipv6Tests
{
    [Theory]
    [InlineData("2001:0db8:85a3:0000:0000:8a2e:0370:7334", true)]
    [InlineData("2001::85a3::7334", false)]
    public void Validate(string input, bool expected)
    {
        Ipv6.TryValidate(input, out var r);
        r.IsValid.ShouldBe(expected);
    }
}
