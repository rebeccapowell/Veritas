using Veritas;
using Veritas.Tax.DE;

public class WIdNrTests
{
    [Theory]
    [InlineData("DE123456788-00001", true)]
    [InlineData("DE123456788-00002", true)]
    [InlineData("DE123456788-99999", true)]
    [InlineData("DE000000000-00001", true)]
    [InlineData("DE123456788-00000", false)]
    [InlineData("DE123456788-0001", false)]
    [InlineData("DE123456788", false)]
    [InlineData("DE123456788–00001", false)]
    [InlineData("DE12345678１-00001", false)]
    [InlineData("DE123456788-0000A", false)]
    public void ChecksFullStructureAndSuffixBounds(string input, bool expected)
        => WIdNr.TryValidateStructure(input, out _).ShouldBe(expected);

    [Fact]
    public void BaseAndActivityFormsAreExplicitAndPreserveZeros()
    {
        WIdNr.TryValidateBaseStructure("de001234567", out var baseResult).ShouldBeTrue();
        baseResult.Value.Value.ShouldBe("DE001234567");
        baseResult.Value.ToString().ShouldBe("DE001234567");
        WIdNr.TryValidateBaseStructure("DE001234567-00001", out _).ShouldBeFalse();
        WIdNr.TryValidateStructure("de001234567-00002", out var full).ShouldBeTrue();
        full.Value.BaseNumber.Value.ShouldBe("DE001234567");
        full.Value.DistinguishingNumber.ShouldBe("00002");
        full.Value.ToString().ShouldBe("DE001234567-00002");
    }

    [Fact]
    public void FormatIllustrationIsNotTreatedAsChecksumEvidence()
    {
        // This API deliberately checks structure only, including the published shape illustration.
        WIdNr.TryValidateStructure("DE123456789-00001", out _).ShouldBeTrue();
        WIdNr.TryValidateBaseStructure("123456788", out _).ShouldBeFalse();
        WIdNr.TryValidateBaseStructure("DE123.456.788", out _).ShouldBeFalse();
        WIdNr.TryValidateStructure(new string('0', 5_000_000), out _).ShouldBeFalse();
    }

    [Fact]
    public void SyntheticStructureGenerationIsExplicitAndReproducible()
    {
        Span<char> first = stackalloc char[17];
        Span<char> second = stackalloc char[17];
        foreach (int suffix in new[] { 1, 2, 99999 })
        {
            for (int seed = 0; seed < 100; seed++)
            {
                var options = new GenerationOptions { Seed = seed };
                WIdNr.TryGenerateSyntheticStructure(options, suffix, first, out int written).ShouldBeTrue();
                written.ShouldBe(17);
                WIdNr.TryValidateStructure(first, out var result).ShouldBeTrue();
                result.Value.DistinguishingNumber.ShouldBe(suffix.ToString("D5", System.Globalization.CultureInfo.InvariantCulture));
                WIdNr.TryGenerateSyntheticStructure(options, suffix, second, out _).ShouldBeTrue();
                first.SequenceEqual(second).ShouldBeTrue();
            }
        }
        for (int size = 0; size < 17; size++)
        {
            WIdNr.TryGenerateSyntheticStructure(default, 1, new char[size], out int written).ShouldBeFalse();
            written.ShouldBe(0);
        }
        foreach (int invalid in new[] { -1, 0, 100000 })
        {
            WIdNr.TryGenerateSyntheticStructure(default, invalid, first, out int written).ShouldBeFalse();
            written.ShouldBe(0);
        }
        WIdNr.TryGenerateSyntheticBaseStructure(new GenerationOptions { Seed = 42 }, first, out int count).ShouldBeTrue();
        count.ShouldBe(11);
        WIdNr.TryValidateBaseStructure(first[..count], out _).ShouldBeTrue();
        WIdNr.TryGenerateSyntheticBaseStructure(new GenerationOptions { Seed = 42 }, second, out count).ShouldBeTrue();
        first[..count].SequenceEqual(second[..count]).ShouldBeTrue();
        for (int size = 0; size < 11; size++)
        {
            WIdNr.TryGenerateSyntheticBaseStructure(default, new char[size], out int written).ShouldBeFalse();
            written.ShouldBe(0);
        }
        WIdNr.TryGenerateSyntheticStructure(first, out count).ShouldBeTrue();
        WIdNr.TryValidateStructure(first[..count], out var initial).ShouldBeTrue();
        initial.Value.DistinguishingNumber.ShouldBe("00001");
    }
}
