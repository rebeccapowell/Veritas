using System;
using Veritas.Standards;
using Xunit;
using Shouldly;

public class IecStandardNumberTests
{
    [Theory]
    [InlineData("IEC 1234", true)]
    [InlineData("IEC1234:2020", true)]
    [InlineData("iec-5678:1999", true)]
    [InlineData("IEC123", false)]
    [InlineData("IEC12345", false)]
    [InlineData("IEC1234:20", false)]
    [InlineData("ABC1234", false)]
    public void Validate(string input, bool expected)
    {
        IecStandardNumber.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[12];
        IecStandardNumber.TryGenerate(buffer, out var written).ShouldBeTrue();
        IecStandardNumber.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

