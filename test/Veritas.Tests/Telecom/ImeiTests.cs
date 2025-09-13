using System;
using Veritas.Telecom;
using Xunit;
using Shouldly;

public class ImeiTests
{
    [Theory]
    [InlineData("490154203237518", true)]
    [InlineData("490154203237519", false)]
    public void Validate(string input, bool expected)
    {
        Imei.TryValidate(input, out var r).ShouldBe(expected);
        r.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[15];
        Imei.TryGenerate(buffer, out var written).ShouldBeTrue();
        Imei.TryValidate(buffer[..written], out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }
}
