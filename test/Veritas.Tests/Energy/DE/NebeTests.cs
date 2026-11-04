using Veritas;
using Veritas.Energy.DE;

public class NebeTests
{
    [Theory]
    [InlineData("F1137355920", true)]
    [InlineData("F0000000000", true)]
    [InlineData("FABCDEFGHI4", true)]
    [InlineData("F1137355925", false)]
    [InlineData("A1137355925", false)]
    [InlineData("F113735592A", false)]
    [InlineData("F113７355920", false)]
    [InlineData("Fſ000000004", false)]
    [InlineData("", false)]
    [InlineData("E1137355921", false)]
    public void ValidatesPrefixAlphabetAndAsciiChecksum(string input, bool expected)
        => Nebe.TryValidate(input, out _).ShouldBe(expected);

    [Fact]
    public void NormalisesLowerCaseAndSpaces()
    {
        Nebe.TryValidate("f 1137355920", out var result).ShouldBeTrue();
        result.Value.Value.ShouldBe("F1137355920");
        result.Value.ToString().ShouldBe(result.Value.Value);
    }

    [Fact]
    public void GenerationIsDeterministicAndIndependentlyChecksummed()
    {
        Span<char> first = stackalloc char[11];
        Span<char> second = stackalloc char[11];
        for (int seed = 0; seed < 200; seed++)
        {
            var options = new GenerationOptions { Seed = seed };
            Nebe.TryGenerate(options, first, out int written).ShouldBeTrue();
            written.ShouldBe(11);
            first[0].ShouldBe('F');
            Nebe.TryValidate(first, out _).ShouldBeTrue();
            Nebe.TryGenerate(options, second, out _).ShouldBeTrue();
            first.SequenceEqual(second).ShouldBeTrue();
            int sum = 0;
            for (int i = 0; i < 10; i++)
            {
                int value = char.IsAsciiDigit(first[i]) ? first[i] - '0' : first[i];
                sum += value * (i % 2 == 0 ? 1 : 2);
            }
            ((sum + first[10] - '0') % 10).ShouldBe(0);
        }
        for (int size = 0; size < 11; size++)
        {
            Nebe.TryGenerate(new char[size], out int written).ShouldBeFalse();
            written.ShouldBe(0);
        }
        Nebe.TryValidate(new string('F', 5_000_000), out _).ShouldBeFalse();
    }

    [Fact]
    public void SharedAsciiChecksumMatchesOfficialResourceExample()
        => BdewChecksum.ComputeAlphanumeric("A113735592").ShouldBe(5);
}
