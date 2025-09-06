using Veritas.Tax.SE;
using Xunit;
using Shouldly;

public class PersonnummerTests
{
    [Theory]
    [InlineData("850709-9805", true)]
    [InlineData("8507099804", false)]
    public void Validate(string input, bool expected)
    {
        Personnummer.TryValidate(input, out var result);
        result.IsValid.ShouldBe(expected);
    }
}
