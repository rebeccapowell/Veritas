using Veritas.Logistics;

namespace Veritas.Tests.Logistics;

public class GiaiTests
{
    [Theory]
    [InlineData("ABC123XYZ789", true)]
    [InlineData("abc123", true)]
    [InlineData("!123", false)]
    public void Validate_Works(string input, bool expected)
    {
        var ok = Giai.TryValidate(input, out var result);
        Assert.Equal(expected, ok);
        Assert.Equal(expected, result.IsValid);
    }

    [Fact]
    public void Generate_ProducesValid()
    {
        Span<char> dest = stackalloc char[12];
        Assert.True(Giai.TryGenerate(dest, out var written));
        Assert.True(Giai.TryValidate(new string(dest[..written]), out _));
    }
}

