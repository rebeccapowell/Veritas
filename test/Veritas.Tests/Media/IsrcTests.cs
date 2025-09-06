using Veritas.Media;
using Xunit;
using Shouldly;

public class IsrcTests
{
    [Theory]
    [InlineData("USRC17607839", true)]
    [InlineData("us-rc1-7607839", true)]
    [InlineData("USRC1760783X", false)]
    [InlineData("USRC1760783", false)]
    public void Validate(string input, bool expected)
    {
        Isrc.TryValidate(input, out var r);
        r.IsValid.ShouldBe(expected);
    }
}
