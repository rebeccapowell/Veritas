using Veritas;
using Veritas.Finance.DE;

public class LeitwegIdTests
{
    [Theory]
    [InlineData("04011000-1234512345-06", true)]
    [InlineData("991-ABC-29", true)]
    [InlineData("991-abc-29", true)]
    [InlineData("04-86", true)]
    [InlineData("99-92", true)]
    [InlineData("04011000-1234512345-07", false)]
    [InlineData("04011000--1234512345-06", false)]
    [InlineData("04011000-1234512345–06", false)]
    [InlineData("0204:04011000-1234512345-06", false)]
    [InlineData("991-A B-29", false)]
    [InlineData("99--92", false)]
    [InlineData("０4-86", false)]
    [InlineData("0401-A-48", false)]
    [InlineData("17-A-12", false)]
    [InlineData("01-A-59", true)]
    [InlineData("010000000000-ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ-86", true)]
    public void ValidatesIndependentFixturesAndSeparators(string input, bool expected)
        => LeitwegId.TryValidate(input, out _).ShouldBe(expected);

    [Fact]
    public void ParsesComponentsAndPreservesZeros()
    {
        LeitwegId.TryValidate("04011000-1234512345-06", out var result).ShouldBeTrue();
        result.Value.CoarseAddress.ShouldBe("04011000");
        result.Value.FineAddress.ShouldBe("1234512345");
        result.Value.CheckDigits.ShouldBe("06");
        result.Value.ToString().ShouldBe("04011000-1234512345-06");
        LeitwegId.TryValidate("04-86", out result).ShouldBeTrue();
        result.Value.FineAddress.ShouldBeNull();
    }

    [Fact]
    public void GeneratesFromExplicitComponents()
    {
        Span<char> buffer = stackalloc char[46];
        LeitwegId.TryGenerate("04011000", "1234512345", buffer, out int written).ShouldBeTrue();
        new string(buffer[..written]).ShouldBe("04011000-1234512345-06");
        LeitwegId.TryGenerate("991", "abc", buffer, out written).ShouldBeTrue();
        new string(buffer[..written]).ShouldBe("991-ABC-29");
        LeitwegId.TryGenerate("04", "", buffer, out written).ShouldBeTrue();
        new string(buffer[..written]).ShouldBe("04-86");
    }

    [Fact]
    public void SupportsAllCoarseLengthsAndOverlappingComponents()
    {
        Span<char> buffer = stackalloc char[46];
        foreach (string coarse in new[] { "04", "991", "04011", "04011000", "040110000", "010000000000" })
        {
            LeitwegId.TryGenerate(coarse, "A", buffer, out int written).ShouldBeTrue();
            LeitwegId.TryValidate(buffer[..written], out _).ShouldBeTrue();
        }
        "040110001234512345".AsSpan().CopyTo(buffer);
        LeitwegId.TryGenerate(buffer[..8], buffer.Slice(8, 10), buffer, out int count).ShouldBeTrue();
        new string(buffer[..count]).ShouldBe("04011000-1234512345-06");
        LeitwegId.TryGenerate("04", "", new char[5], out count).ShouldBeTrue();
        count.ShouldBe(5);
        LeitwegId.TryValidate("04011000-1234512345-07", out var badChecksum).ShouldBeFalse();
        badChecksum.Error.ShouldBe(ValidationError.Checksum);
        LeitwegId.TryValidate("0401-A-48", out var badFormat).ShouldBeFalse();
        badFormat.Error.ShouldBe(ValidationError.Format);
        foreach (bool includeFine in new[] { false, true })
        {
            for (int size = 0; size < (includeFine ? 15 : 5); size++)
            {
                LeitwegId.TryGenerate(default, includeFine, new char[size], out count).ShouldBeFalse();
                count.ShouldBe(0);
            }
        }
    }

    [Theory]
    [InlineData("00")]
    [InlineData("17")]
    [InlineData("0401")]
    [InlineData("040110")]
    [InlineData("0401100")]
    [InlineData("0401100000")]
    public void RejectsInvalidCoarseComponents(string coarse)
    {
        LeitwegId.TryGenerate(coarse, "A", new char[46], out int written).ShouldBeFalse();
        written.ShouldBe(0);
    }

    [Fact]
    public void HandlesMaximumComponentsAndShortBuffers()
    {
        Span<char> buffer = stackalloc char[46];
        string fine = new string('Z', 30);
        LeitwegId.TryGenerate("010000000000", fine, buffer, out int written).ShouldBeTrue();
        written.ShouldBe(46);
        LeitwegId.TryValidate(buffer, out _).ShouldBeTrue();
        for (int size = 0; size < written; size++)
        {
            LeitwegId.TryGenerate("010000000000", fine, new char[size], out int count).ShouldBeFalse();
            count.ShouldBe(0);
        }
        LeitwegId.TryGenerate("01", new string('A', 31), buffer, out written).ShouldBeFalse();
        written.ShouldBe(0);
        LeitwegId.TryGenerate("01", "ſ", buffer, out written).ShouldBeFalse();
        written.ShouldBe(0);
        LeitwegId.TryValidate(new string('1', 5_000_000), out _).ShouldBeFalse();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void SeededGenerationSupportsOptionalFineAddress(bool includeFine)
    {
        Span<char> first = stackalloc char[46];
        Span<char> second = stackalloc char[46];
        for (int seed = 0; seed < 100; seed++)
        {
            var options = new GenerationOptions { Seed = seed };
            LeitwegId.TryGenerate(options, includeFine, first, out int written).ShouldBeTrue();
            LeitwegId.TryValidate(first[..written], out var result).ShouldBeTrue();
            (result.Value.FineAddress is not null).ShouldBe(includeFine);
            LeitwegId.TryGenerate(options, includeFine, second, out int otherWritten).ShouldBeTrue();
            otherWritten.ShouldBe(written);
            first[..written].SequenceEqual(second[..written]).ShouldBeTrue();
        }
    }
}
