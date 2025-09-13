using Veritas.Tax.FR;
using Xunit;
using Shouldly;

public class SiretTests
{
    [Theory]
    [InlineData("55210055400013", true)]
    [InlineData("55210055400014", false)]
    public void Validate(string input, bool expected)
    {
        Siret.TryValidate(input, out var r).ShouldBe(expected);
        r.IsValid.ShouldBe(expected);
    }
}

