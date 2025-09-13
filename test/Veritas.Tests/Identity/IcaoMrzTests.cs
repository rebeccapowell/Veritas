using Veritas.Identity;

namespace Veritas.Tests.Identity;

public class IcaoMrzTests
{
    private const string Sample = "P<UTOERIKSSON<<ANNA<MARIA<<<<<<<<<<<<<<<<<<<\nL898902C36UTO7408122F1204159ZE184226B<<<<<10";

    [Theory]
    [InlineData(Sample, true)]
    [InlineData("P<UTOERIKSSON<<ANNA<MARIA<<<<<<<<<<<<<<<<<<<\nL898902C36UTO7408122F1204159ZE184226B<<<<<11", false)]
    public void Validate_Works(string input, bool expected)
    {
        var ok = IcaoMrz.TryValidate(input, out var result);
        Assert.Equal(expected, ok);
        Assert.Equal(expected, result.IsValid);
    }

    [Fact]
    public void Generate_ProducesValid()
    {
        Span<char> dest = stackalloc char[89];
        Assert.True(IcaoMrz.TryGenerate(dest, out var written));
        Assert.True(IcaoMrz.TryValidate(new string(dest[..written]), out _));
    }
}

