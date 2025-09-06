using Veritas.Telecom;
using Xunit;
using Shouldly;

public class MeidTests
{
    [Theory]
    [InlineData("A0000000002322", true)]
    [InlineData("A000000000232Z", false)]
    [InlineData("A000000000232", false)]
    public void Validate(string input, bool expected)
    {
        Meid.TryValidate(input, out var result);
        result.IsValid.ShouldBe(expected);
    }
}
