using Veritas.Tax.PL;
using Xunit;
using Shouldly;

public class PeselTests
{
    [Theory]
    [InlineData("02070803628", true)]
    [InlineData("44051401358", false)]
    public void Validate(string input, bool expected)
    {
        Pesel.TryValidate(input, out var result);
        result.IsValid.ShouldBe(expected);
    }
}
