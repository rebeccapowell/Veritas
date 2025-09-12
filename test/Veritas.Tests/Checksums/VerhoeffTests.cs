using Veritas.Checksums;
using Xunit;
using Shouldly;

public class VerhoeffTests
{
    [Fact]
    public void Compute_Sample()
    {
        Verhoeff.Compute("2363").ShouldBe((byte)4);
    }

    [Theory]
    [InlineData("23634", true)]
    [InlineData("23635", false)]
    public void Validate_Sample(string input, bool expected)
    {
        Verhoeff.Validate(input).ShouldBe(expected);
    }

    [Fact]
    public void Append_AppendsCheck()
    {
        Span<char> dest = stackalloc char[5];
        Verhoeff.Append("2363", dest, out var written).ShouldBeTrue();
        new string(dest[..written]).ShouldBe("23634");
    }
}
