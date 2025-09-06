using System;
using Veritas.Logistics;
using Xunit;
using Shouldly;

public class ImoTests
{
    [Theory]
    [InlineData("IMO9319466", true)]
    [InlineData("9319466", true)]
    [InlineData("IMO9319467", false)]
    [InlineData("123456", false)]
    public void Validate(string input, bool expected)
    {
        Imo.TryValidate(input, out var r);
        r.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[10];
        Imo.TryGenerate(buffer, out var written).ShouldBeTrue();
        Imo.TryValidate(buffer[..written], out var r);
        r.IsValid.ShouldBeTrue();
    }
}
