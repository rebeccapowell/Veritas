using Veritas;
using Veritas.Energy.DE;

public class MarketPartnerIdTests
{
    [Fact]
    public void ExpectedSchemePreservesChecksumError()
    {
        MarketPartnerId.TryValidate("9900000000004", out var basic).ShouldBeFalse();
        MarketPartnerId.TryValidate("9900000000004", MarketPartnerScheme.BdewElectricity, out var specific).ShouldBeFalse();
        basic.Error.ShouldBe(ValidationError.Checksum);
        specific.Error.ShouldBe(basic.Error);
    }

    [Theory]
    [InlineData("9900000000003", MarketPartnerScheme.BdewElectricity, true)]
    [InlineData("9800000000005", MarketPartnerScheme.DvgwGas, true)]
    [InlineData("9912345678908", MarketPartnerScheme.BdewElectricity, true)]
    [InlineData("9891234567896", MarketPartnerScheme.DvgwGas, true)]
    [InlineData("9991234567894", MarketPartnerScheme.BdewElectricity, false)]
    [InlineData("4901234567894", MarketPartnerScheme.BdewElectricity, false)]
    [InlineData("9900000000003", MarketPartnerScheme.DvgwGas, false)]
    [InlineData("９９00000000003", MarketPartnerScheme.BdewElectricity, false)]
    public void ValidatesSchemeAndChecksum(string input, MarketPartnerScheme scheme, bool expected)
        => MarketPartnerId.TryValidate(input, scheme, out _).ShouldBe(expected);

    [Fact]
    public void NormalisesAndClassifies()
    {
        MarketPartnerId.TryValidate("99 0000 0000 003", out var result).ShouldBeTrue();
        result.Value.Value.ShouldBe("9900000000003");
        result.Value.Scheme.ShouldBe(MarketPartnerScheme.BdewElectricity);
        result.Value.AllocationMode.ShouldBe(0);
        result.Value.ToString().ShouldBe(result.Value.Value);
    }

    [Theory]
    [InlineData(MarketPartnerScheme.BdewElectricity)]
    [InlineData(MarketPartnerScheme.DvgwGas)]
    public void GeneratesDeterministicSyntheticExamples(MarketPartnerScheme scheme)
    {
        Span<char> first = stackalloc char[13];
        Span<char> second = stackalloc char[13];
        for (int seed = 0; seed < 200; seed++)
        {
            var options = new GenerationOptions { Seed = seed };
            MarketPartnerId.TryGenerate(scheme, options, first, out int written).ShouldBeTrue();
            written.ShouldBe(13);
            MarketPartnerId.TryValidate(first, scheme, out _).ShouldBeTrue();
            MarketPartnerId.TryGenerate(scheme, options, second, out _).ShouldBeTrue();
            first.SequenceEqual(second).ShouldBeTrue();
            int sum = 0;
            for (int i = 0; i < 12; i++) sum += (first[i] - '0') * (i % 2 == 0 ? 1 : 2);
            ((sum + first[12] - '0') % 10).ShouldBe(0);
        }
        for (int size = 0; size < 13; size++)
        {
            MarketPartnerId.TryGenerate(scheme, new char[size], out int written).ShouldBeFalse();
            written.ShouldBe(0);
        }
        MarketPartnerId.TryGenerate((MarketPartnerScheme)99, first, out int invalidWritten).ShouldBeFalse();
        invalidWritten.ShouldBe(0);
    }

    [Fact]
    public void SharedChecksumRetainsOfficialMaloVector()
        => Malo.TryValidate("41373559241", out _).ShouldBeTrue();
}
