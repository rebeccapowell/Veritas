using Veritas.Tax.US;
using Xunit;
using Shouldly;

public class EinTests
{
    [Theory]
    [InlineData("12-3456789", true)]
    [InlineData("00-1234567", false)]
    public void Validate(string input, bool expected)
    {
        Ein.TryValidate(input, out var r);
        r.IsValid.ShouldBe(expected);
    }
}

