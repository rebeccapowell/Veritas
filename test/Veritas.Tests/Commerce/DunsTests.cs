using Veritas;
using Veritas.Commerce;

public class DunsTests
{
    [Theory]
    [InlineData("123456789", "123456789")]
    [InlineData("12-345-6789", "123456789")]
    [InlineData("000123456", "000123456")]
    [InlineData("00-012-3456", "000123456")]
    [InlineData("000000000", "000000000")]
    public void ValidatesStructureAndPreservesZeros(string input, string canonical)
    {
        // Structural fixtures make no assertion about D&B assignment or company identity.
        Duns.TryValidate(input, out var result).ShouldBeTrue();
        result.Value.Value.ShouldBe(canonical);
        result.Value.ToString().ShouldBe(canonical);
    }

    [Theory]
    [InlineData("")]
    [InlineData("12345678")]
    [InlineData("1234567890")]
    [InlineData("12 345 6789")]
    [InlineData("1-2345-6789")]
    [InlineData("123-45-6789")]
    [InlineData("12.345.6789")]
    [InlineData("12-3456789")]
    [InlineData("１２３４５６７８９")]
    [InlineData("١٢٣٤٥٦٧٨٩")]
    [InlineData("12345678A")]
    [InlineData(" 123456789")]
    public void RejectsOtherFormattingAndNonAsciiDigits(string input)
        => Duns.TryValidate(input, out _).ShouldBeFalse();

    [Fact]
    public void DoesNotInventALuhnRequirement()
    {
        Veritas.Algorithms.Luhn.Validate("123456789").ShouldBeFalse();
        Duns.TryValidate("123456789", out _).ShouldBeTrue();
    }

    [Fact]
    public void GenerationIsDeterministicAndHandlesAllShortBuffers()
    {
        Span<char> first = stackalloc char[9];
        Span<char> second = stackalloc char[9];
        for (int seed = 0; seed < 200; seed++)
        {
            var options = new GenerationOptions { Seed = seed };
            Duns.TryGenerate(options, first, out int written).ShouldBeTrue();
            written.ShouldBe(9);
            Duns.TryValidate(first, out _).ShouldBeTrue();
            for (int i = 0; i < written; i++) char.IsAsciiDigit(first[i]).ShouldBeTrue();
            Duns.TryGenerate(options, second, out _).ShouldBeTrue();
            first.SequenceEqual(second).ShouldBeTrue();
        }
        for (int size = 0; size < 9; size++)
        {
            Duns.TryGenerate(new char[size], out int written).ShouldBeFalse();
            written.ShouldBe(0);
        }
        Duns.TryValidate(new string('1', 5_000_000), out _).ShouldBeFalse();
    }
}
