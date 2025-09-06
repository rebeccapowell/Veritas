using Veritas.Tax.FR;
using Xunit;
using Shouldly;

public class SirenTests
{
    [Theory]
    [InlineData("552100554", true)]
    [InlineData("552100555", false)]
    public void Validate(string input, bool expected)
    {
        Siren.TryValidate(input, out var r);
        r.IsValid.ShouldBe(expected);
    }
}

