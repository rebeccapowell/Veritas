using Veritas.Tax.UK;
using Xunit;
using Shouldly;

public class UtrTests
{
    [Theory]
    [InlineData("1123456789", true)]
    [InlineData("2123456789", false)]
    public void Validate(string input, bool expected)
    {
        Utr.TryValidate(input, out var r);
        r.IsValid.ShouldBe(expected);
    }
}

