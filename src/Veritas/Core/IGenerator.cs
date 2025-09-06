using System;
namespace Veritas;

/// <summary>Generates identifiers for testing or reserved ranges.</summary>
/// <typeparam name="T">Strong type the generator emits.</typeparam>
public interface IGenerator<T>
{
    bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written);
}
