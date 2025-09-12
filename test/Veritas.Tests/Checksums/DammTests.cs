using Veritas.Checksums;
using Xunit;
using Shouldly;

public class DammTests
{
    [Fact]
    public void Compute_Sample()
    {
        Damm.Compute("572").ShouldBe((byte)4);
    }

    [Theory]
    [InlineData("5724", true)]
    [InlineData("5727", false)]
    public void Validate_Sample(string input, bool expected)
    {
        Damm.Validate(input).ShouldBe(expected);
    }
}
