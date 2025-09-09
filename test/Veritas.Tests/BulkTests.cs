using System.Linq;
using Veritas;

public class BulkTests
{
    [Fact]
    public void GenerateMany_ProducesDeterministicResults()
    {
        static (bool ok, int written) Gen(Span<char> dst, Random rng)
        {
            dst[0] = (char)('0' + rng.Next(10));
            return (true, 1);
        }

        var first = Bulk.GenerateMany(Gen, 5, seed: 123).ToArray();
        var second = Bulk.GenerateMany(Gen, 5, seed: 123).ToArray();

        first.ShouldBe(second);
    }

    [Fact]
    public void GenerateMany_NegativeCount_Throws()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => Bulk.GenerateMany((_, __) => (true, 0), -1).ToArray());
    }
}
