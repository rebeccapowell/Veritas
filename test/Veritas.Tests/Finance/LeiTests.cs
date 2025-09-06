using Veritas.Finance;
using Xunit;
using Shouldly;

public class LeiTests
{
    [Theory]
    [InlineData("PPOT00HKB2SNV4KKTR81", true)]
    [InlineData("PPOT00HKB2SNV4KKTR82", false)]
    [InlineData("PPOT00HKB2SNV4KKT", false)]
    public void Validate(string input, bool expected)
    {
        Lei.TryValidate(input, out var result);
        result.IsValid.ShouldBe(expected);
    }
}
