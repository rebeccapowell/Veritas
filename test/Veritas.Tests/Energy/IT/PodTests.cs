using Veritas.Energy.IT;
using Xunit;
using Shouldly;

public class PodTests
{
    [Theory]
    [InlineData("IT123ABCDE123456", true)]
    [InlineData("FR123ABCDE123456", false)]
    public void Validate_Works(string input, bool expected)
    {
        Pod.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }
}
