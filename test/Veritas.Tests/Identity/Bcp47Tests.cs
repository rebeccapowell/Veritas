using Veritas.Identity;
using Xunit;
using Shouldly;

public class Bcp47Tests
{
    [Theory]
    [InlineData("en-US", true)]
    [InlineData("en_us", false)]
    public void Validate(string tag, bool expected)
    {
        Bcp47.TryValidate(tag, out var r).ShouldBe(expected);
        r.IsValid.ShouldBe(expected);
    }
}
