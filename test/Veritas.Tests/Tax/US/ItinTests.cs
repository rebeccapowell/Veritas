using Veritas.Tax.US;
using Xunit;
using Shouldly;

public class ItinTests
{
    [Theory]
    [InlineData("912-90-3456", true)]
    [InlineData("912-93-4567", false)]
    public void Validate(string input, bool expected)
    {
        Itin.TryValidate(input, out var r);
        r.IsValid.ShouldBe(expected);
    }
}

