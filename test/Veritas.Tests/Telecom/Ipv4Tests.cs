using Veritas.Telecom;
using Xunit;
using Shouldly;

public class Ipv4Tests
{
    [Theory]
    [InlineData("192.168.0.1", true)]
    [InlineData("256.0.0.1", false)]
    public void Validate(string input, bool expected)
    {
        Ipv4.TryValidate(input, out var r).ShouldBe(expected);
        r.IsValid.ShouldBe(expected);
    }
}
