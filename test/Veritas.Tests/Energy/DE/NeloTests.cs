using Veritas;
using Veritas.Energy.DE;

public class NeloTests
{
    [Theory]
    [InlineData("E1137355921", true)]
    [InlineData("E0000000001", true)]
    [InlineData("EABCDEFGHI5", true)]
    [InlineData("E1137355925", false)]
    [InlineData("A1137355925", false)]
    [InlineData("E113735592A", false)]
    [InlineData("E113７355921", false)]
    [InlineData("Eſ000000005", false)]
    [InlineData("", false)]
    public void ValidatesPrefixAlphabetAndAsciiChecksum(string input, bool expected)
        => Nelo.TryValidate(input, out _).ShouldBe(expected);

    [Fact]
    public void NormalisesLowerCaseAndSpaces()
    {
        Nelo.TryValidate("e 1137355921", out var result).ShouldBeTrue();
        result.Value.Value.ShouldBe("E1137355921");
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
            Nelo.TryGenerate(options, first, out int written).ShouldBeTrue();
            written.ShouldBe(11);
            first[0].ShouldBe('E');
            Nelo.TryValidate(first, out _).ShouldBeTrue();
            Nelo.TryGenerate(options, second, out _).ShouldBeTrue();
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
            Nelo.TryGenerate(new char[size], out int written).ShouldBeFalse();
            written.ShouldBe(0);
        }
        Nelo.TryValidate(new string('E', 5_000_000), out _).ShouldBeFalse();
    }

    [Fact]
    public void SharedAsciiChecksumMatchesOfficialResourceExample()
        => BdewChecksum.ComputeAlphanumeric("A113735592").ShouldBe(5);
}
