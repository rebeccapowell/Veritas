using System;
using Veritas.Tax.BR;
using Xunit;
using Shouldly;

public class CnpjTests
{
    [Theory]
    [InlineData("04.252.011/0001-10", true)]
    [InlineData("04.252.011/0001-11", false)]
    public void Validate_Works(string input, bool expected)
    {
        Cnpj.TryValidate(input, out var result).ShouldBe(expected);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[14];
        Cnpj.TryGenerate(buffer, out var written).ShouldBeTrue();
        Cnpj.TryValidate(buffer[..written], out var result).ShouldBeTrue();
        result.IsValid.ShouldBeTrue();
    }
}

