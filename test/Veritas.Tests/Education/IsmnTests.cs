using System;
using Veritas.Education;
using Xunit;
using Shouldly;

public class IsmnTests
{
    [Theory]
    [InlineData("M123456785", true)]
    [InlineData("M123456786", false)]
    [InlineData("9790000000001", true)]
    [InlineData("9790000000000", false)]
    [InlineData("M12345", false)]
    public void Validate(string input, bool expected)
    {
        Ismn.TryValidate(input, out var r);
        r.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[13];
        Ismn.TryGenerate(buffer, out var written).ShouldBeTrue();
        Ismn.TryValidate(buffer[..written], out var r);
        r.IsValid.ShouldBeTrue();
    }
}
