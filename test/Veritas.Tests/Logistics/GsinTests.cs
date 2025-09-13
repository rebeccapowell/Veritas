using Shouldly;
using Veritas.Logistics;
using Xunit;

namespace Veritas.Tests.Logistics;

public class GsinTests
{
    [Theory]
    [InlineData("12345678901234560", true)]
    [InlineData("12345678901234561", false)]
    [InlineData("1234567890123456", false)]
    [InlineData("1234567890123456A", false)]
    public void Validate_Works(string input, bool expected)
        => Gsin.TryValidate(input, out var r).ShouldBe(expected);

    [Fact]
    public void Generate_RoundTrips()
    {
        Span<char> buffer = stackalloc char[17];
        Gsin.TryGenerate(buffer, out var written).ShouldBeTrue();
        Gsin.TryValidate(buffer[..written], out var r).ShouldBeTrue();
    }
}
