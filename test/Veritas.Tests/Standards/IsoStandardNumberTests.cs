using System;
using Veritas.Standards;
using Xunit;
using Shouldly;

public class IsoStandardNumberTests
{
    [Theory]
    [InlineData("ISO 1234", true)]
    [InlineData("ISO1234:2020", true)]
    [InlineData("iso-5678:1999", true)]
    [InlineData("ISO123", false)]
    [InlineData("ISO12345", false)]
    [InlineData("ISO1234:20", false)]
    [InlineData("ABC1234", false)]
    public void Validate(string input, bool expected)
    {
        IsoStandardNumber.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[12];
        IsoStandardNumber.TryGenerate(buffer, out var written).ShouldBeTrue();
        IsoStandardNumber.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

