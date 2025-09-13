using Veritas.Logistics;

namespace Veritas.Tests.Logistics;

public class UpuS10Tests
{
    [Theory]
    [InlineData("RA123456785GB", true)]
    [InlineData("RA123456785US", true)]
    [InlineData("RA123456784GB", false)]
    public void Validate_Works(string input, bool expected)
    {
        var ok = UpuS10.TryValidate(input, out var result);
        Assert.Equal(expected, ok);
        Assert.Equal(expected, result.IsValid);
    }

    [Fact]
    public void Generate_ProducesValid()
    {
        Span<char> dest = stackalloc char[13];
        Assert.True(UpuS10.TryGenerate(dest, out var written));
        Assert.True(UpuS10.TryValidate(new string(dest[..written]), out _));
    }
}

