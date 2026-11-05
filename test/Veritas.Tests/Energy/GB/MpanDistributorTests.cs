using Veritas;
using Veritas.Energy.GB;

public class MpanDistributorTests
{
    [Theory]
    [InlineData("0000000000000", false)]
    [InlineData("0900000000001", false)]
    [InlineData("1000000000003", true)]
    [InlineData("2900000000007", true)]
    [InlineData("3000000000009", true)]
    public void RejectsZeroPrefixedDistributorWithoutFreezingOldUpperBound(string input, bool expected)
        => Mpan.TryValidate(input, out _).ShouldBe(expected);

    [Fact]
    public void SyntheticGenerationUsesAKnownDistributorFormat()
    {
        Span<char> buffer = stackalloc char[13];
        for (int seed = 0; seed < 1000; seed++)
        {
            Mpan.TryGenerate(new GenerationOptions { Seed = seed }, buffer, out int written).ShouldBeTrue();
            int distributor = (buffer[0] - '0') * 10 + buffer[1] - '0';
            distributor.ShouldBeInRange(10, 30);
            Mpan.TryValidate(buffer[..written], out _).ShouldBeTrue();
        }
        for (int size = 0; size < 13; size++)
        {
            Mpan.TryGenerate(new char[size], out int written).ShouldBeFalse();
            written.ShouldBe(0);
        }
    }
}
