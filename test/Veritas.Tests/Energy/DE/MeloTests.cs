using Veritas.Energy.DE;
using Xunit;
using Shouldly;

public class MeloTests
{
    [Theory]
    [InlineData("DEDGX4QK0NIAD8KYGGK09X58Y0Q4HLFEB", true)]
    [InlineData("XEDGX4QK0NIAD8KYGGK09X58Y0Q4HLFEB", false)]
    [InlineData("DEDGX4QK0NIAD8KYGGK09X58Y0Q4HLFE", false)]
    public void Validate(string input, bool expected)
    {
        Melo.TryValidate(input, out var result);
        result.IsValid.ShouldBe(expected);
    }
}
