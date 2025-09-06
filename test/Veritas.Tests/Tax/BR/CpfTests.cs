using System;
using Veritas.Tax.BR;
using Xunit;
using Shouldly;

public class CpfTests
{
    [Theory]
    [InlineData("111.444.777-35", true)]
    [InlineData("111.444.777-36", false)]
    public void Validate_Works(string input, bool expected)
    {
        Cpf.TryValidate(input, out var result);
        result.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[11];
        Cpf.TryGenerate(buffer, out var written).ShouldBeTrue();
        Cpf.TryValidate(buffer[..written], out var result);
        result.IsValid.ShouldBeTrue();
    }
}

