using Veritas.Logistics;
using Xunit;
using Shouldly;

public class VinTests
{
    [Theory]
    [InlineData("1HGCM82633A004352", true)]
    [InlineData("1HGCM82633A004353", false)]
    public void Validate(string input, bool expected)
    {
        Vin.TryValidate(input, out var r).ShouldBe(expected);
        r.IsValid.ShouldBe(expected);
    }
}
