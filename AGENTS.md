# AGENTS

## Repository-wide Guidelines

- Use `rg` for searching the codebase.
- Run `dotnet test` before every commit.
- When C# files are changed, also run `dotnet format` to ensure style compliance.
- Keep documentation in sync: update `README.md` and `docfx/index.md` when adding or changing identifiers.
- Public APIs must include XML documentation comments.
- Favor span-based, allocation-free implementations and `TryValidate`/`TryGenerate` patterns.
- PR descriptions must include a **Summary** of changes and a **Testing** section with executed commands.

## Definition of Done

- Each identifier ships with a parser/normalizer and deterministic checksum.
- Generators produce valid examples using `GenerationOptions` and are covered by positive/negative tests.
- Public APIs include XML docs with a one-line summary and example usage.
- `README.md` and `docfx/index.md` list newly added identifiers and DocFX builds cleanly.

## Coding Style

- Prefer `ReadOnlySpan<char>`/`Span<char>` over `string` to minimize allocations.
- Expose `TryValidate`/`TryGenerate` methods returning `bool` and `out` parameters for normalized values.
- Implement value types as `readonly struct` with canonical `ToString()` representations.
- Avoid LINQ and heap allocations in hot paths; use simple loops and `stackalloc` where appropriate.
