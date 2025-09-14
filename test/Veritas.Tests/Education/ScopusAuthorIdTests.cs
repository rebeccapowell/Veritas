using System;
using Veritas.Education;
using Xunit;
using Shouldly;

public class ScopusAuthorIdTests
{
    [Theory]
    [InlineData("12345678901", true)]
    [InlineData("1234567890a", false)]
    [InlineData("12345", false)]
    public void Validate(string input, bool expected)
    {
        ScopusAuthorId.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[11];
        ScopusAuthorId.TryGenerate(buffer, out var written).ShouldBeTrue();
        ScopusAuthorId.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}
