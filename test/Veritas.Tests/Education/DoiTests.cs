using Veritas.Education;
using Xunit;
using Shouldly;

public class DoiTests
{
    [Theory]
    [InlineData("10.1000/182", true)]
    [InlineData("11.1000/182", false)]
    public void Validate(string input, bool expected)
    {
        Doi.TryValidate(input, out var r).ShouldBe(expected);
        r.IsValid.ShouldBe(expected);
    }
}
