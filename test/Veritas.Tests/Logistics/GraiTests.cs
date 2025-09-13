using Shouldly;
using Veritas.Logistics;
using Xunit;

namespace Veritas.Tests.Logistics;

public class GraiTests
{
    [Theory]
    [InlineData("12345678901231", true)]
    [InlineData("12345678901232", false)]
    [InlineData("1234567890123", false)]
    [InlineData("1234567890123A", false)]
    public void Validate_Works(string input, bool expected)
        => Grai.TryValidate(input, out var r).ShouldBe(expected);

    [Fact]
    public void Generate_RoundTrips()
    {
        Span<char> buffer = stackalloc char[14];
        Grai.TryGenerate(buffer, out var written).ShouldBeTrue();
        Grai.TryValidate(buffer[..written], out var r).ShouldBeTrue();
    }
}
