using System;
using Veritas.Standards;
using Xunit;
using Shouldly;

public class RfcNumberTests
{
    [Theory]
    [InlineData("RFC1234", true)]
    [InlineData("RFC 1234", true)]
    [InlineData("rfc-42", true)]
    [InlineData("RFC", false)]
    [InlineData("ABC1234", false)]
    public void Validate(string input, bool expected)
    {
        RfcNumber.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[8];
        RfcNumber.TryGenerate(buffer, out var written).ShouldBeTrue();
        RfcNumber.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}
