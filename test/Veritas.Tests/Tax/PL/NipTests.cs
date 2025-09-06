using Veritas.Tax.PL;
using Xunit;
using Shouldly;

public class NipTests
{
    [Theory]
    [InlineData("6111780027", true)]
    [InlineData("6111780028", false)]
    public void Validate(string input, bool expected)
    {
        Nip.TryValidate(input, out var result);
        result.IsValid.ShouldBe(expected);
    }
}
