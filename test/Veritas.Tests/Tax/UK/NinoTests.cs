using Veritas.Tax.UK;
using Xunit;
using Shouldly;

public class NinoTests
{
    [Theory]
    [InlineData("AA123456A", true)]
    [InlineData("DQ123456A", false)]
    public void Validate(string input, bool expected)
    {
        Nino.TryValidate(input, out var r).ShouldBe(expected);
        r.IsValid.ShouldBe(expected);
    }
}
