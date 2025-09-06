using Veritas.Logistics;
using Xunit;
using Shouldly;

public class Iso6346Tests
{
    [Theory]
    [InlineData("MSCU6639870", true)]
    [InlineData("MSCU6639871", false)]
    public void Validate(string input, bool expected)
    {
        Iso6346.TryValidate(input, out var r);
        r.IsValid.ShouldBe(expected);
    }
}
