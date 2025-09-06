using Veritas.Telecom;
using Xunit;
using Shouldly;

public class MacTests
{
    [Theory]
    [InlineData("00:1A:2B:3C:4D:5E", true)]
    [InlineData("00:1A:2B:3C:4D:5G", false)]
    public void Validate(string input, bool expected)
    {
        Mac.TryValidate(input, out var r);
        r.IsValid.ShouldBe(expected);
    }
}
