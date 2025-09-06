using Veritas.Healthcare;
using Xunit;
using Shouldly;

public class OrcidTests
{
    [Theory]
    [InlineData("0000-0002-1825-0097", true)]
    [InlineData("0000-0002-1825-0098", false)]
    public void Validate(string input, bool expected)
    {
        Orcid.TryValidate(input, out var r);
        r.IsValid.ShouldBe(expected);
    }
}
