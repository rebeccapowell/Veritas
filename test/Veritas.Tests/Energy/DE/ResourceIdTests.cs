using Veritas;
using Veritas.Energy.DE;

public class ResourceIdTests
{
    [Theory]
    [InlineData("A1137355925", ResourceIdType.Cluster)]
    [InlineData("B1137355924", ResourceIdType.ControlGroup)]
    [InlineData("C1137355923", ResourceIdType.Controllable)]
    [InlineData("D1137355922", ResourceIdType.Technical)]
    [InlineData("AABCDEFGHI9", ResourceIdType.Cluster)]
    [InlineData("BABCDEFGHI8", ResourceIdType.ControlGroup)]
    [InlineData("CABCDEFGHI7", ResourceIdType.Controllable)]
    [InlineData("DABCDEFGHI6", ResourceIdType.Technical)]
    public void ValidatesAndClassifiesIndependentVectors(string input, ResourceIdType type)
    {
        ResourceId.TryValidate(input, type, out var result).ShouldBeTrue();
        result.Value.Type.ShouldBe(type);
        result.Value.Value.ShouldBe(input);
        result.Value.ToString().ShouldBe(input);
        foreach (ResourceIdType other in Enum.GetValues<ResourceIdType>())
            ResourceId.TryValidate(input, other, out _).ShouldBe(other == type);
    }

    [Theory]
    [InlineData("A1137355924")]
    [InlineData("E1137355921")]
    [InlineData("F1137355920")]
    [InlineData("D113735592A")]
    [InlineData("D113７355922")]
    [InlineData("Dſ000000006")]
    [InlineData("")]
    public void RejectsWrongChecksumAlphabetAndNonResourceTypes(string input)
        => ResourceId.TryValidate(input, out _).ShouldBeFalse();

    [Fact]
    public void TypedValidationPreservesChecksumErrors()
    {
        ResourceId.TryValidate("A1137355924", ResourceIdType.Cluster, out var result).ShouldBeFalse();
        result.Error.ShouldBe(ValidationError.Checksum);
        ResourceId.TryValidate("d 1137355922", out result).ShouldBeTrue();
        result.Value.Value.ShouldBe("D1137355922");
    }

    [Fact]
    public void SyntheticGenerationHandlesAllTypesAndBuffers()
    {
        Span<char> first = stackalloc char[11];
        Span<char> second = stackalloc char[11];
        foreach (ResourceIdType type in Enum.GetValues<ResourceIdType>())
        {
            for (int seed = 0; seed < 200; seed++)
            {
                var options = new GenerationOptions { Seed = seed };
                ResourceId.TryGenerate(type, options, first, out int written).ShouldBeTrue();
                written.ShouldBe(11);
                ResourceId.TryValidate(first, type, out _).ShouldBeTrue();
                ResourceId.TryGenerate(type, options, second, out _).ShouldBeTrue();
                first.SequenceEqual(second).ShouldBeTrue();
                int sum = 0;
                for (int i = 0; i < 10; i++) sum += (char.IsAsciiDigit(first[i]) ? first[i] - '0' : first[i]) * (i % 2 == 0 ? 1 : 2);
                ((sum + first[10] - '0') % 10).ShouldBe(0);
            }
            for (int size = 0; size < 11; size++)
            {
                ResourceId.TryGenerate(type, new char[size], out int written).ShouldBeFalse();
                written.ShouldBe(0);
            }
        }
        ResourceId.TryGenerate((ResourceIdType)99, first, out int badWritten).ShouldBeFalse();
        badWritten.ShouldBe(0);
        ResourceId.TryValidate(new string(' ', 5_000_000), out _).ShouldBeFalse();
    }
}
