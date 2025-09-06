using System;
using Veritas.Media;
using Xunit;
using Shouldly;

public class IssnTests
{
    [Theory]
    [InlineData("0378-5955", true)]
    [InlineData("0378-5954", false)]
    public void Validate(string input, bool expected)
    {
        Issn.TryValidate(input, out var r);
        r.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[8];
        Issn.TryGenerate(buffer, out var written).ShouldBeTrue();
        Issn.TryValidate(buffer[..written], out var r);
        r.IsValid.ShouldBeTrue();
    }
}
