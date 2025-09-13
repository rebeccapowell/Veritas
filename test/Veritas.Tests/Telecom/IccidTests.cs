using System;
using Veritas.Telecom;
using Xunit;
using Shouldly;

public class IccidTests
{
    [Theory]
    [InlineData("8914800000000000006", true)]
    [InlineData("8914800000000000001", false)]
    public void Validate(string input, bool expected)
    {
        Iccid.TryValidate(input, out var r).ShouldBe(expected);
        r.IsValid.ShouldBe(expected);
    }

    [Fact]
    public void GenerateProducesValid()
    {
        Span<char> buffer = stackalloc char[20];
        Iccid.TryGenerate(buffer, out var written).ShouldBeTrue();
        Iccid.TryValidate(buffer[..written], out var r).ShouldBeTrue();
        r.IsValid.ShouldBeTrue();
    }
}
