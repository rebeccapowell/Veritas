using System;
namespace Veritas;

/// <summary>Generates identifiers for testing or reserved ranges.</summary>
/// <typeparam name="T">Strong type the generator emits.</typeparam>
public interface IGenerator<T>
{
    /// <summary>Attempts to generate an identifier using the supplied options.</summary>
    /// <param name="options">Options controlling identifier generation.</param>
    /// <param name="destination">Buffer to receive the generated value.</param>
    /// <param name="written">When the method returns, contains the number of characters produced.</param>
    /// <returns><c>true</c> if generation succeeded; otherwise, <c>false</c>.</returns>
    bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written);
}
