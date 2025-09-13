# AGENTS

## Repository-wide Guidelines

- Use `rg` for searching the codebase.
- Run `dotnet test` before every commit.
- When C# files are changed, also run `dotnet format` to ensure style compliance.
- Keep documentation in sync: update `README.md` and `docfx/index.md` when adding or changing identifiers.
- Public APIs must include XML documentation comments.
- Favor span-based, allocation-free implementations and `TryValidate`/`TryGenerate` patterns.
- PR descriptions must include a **Summary** of changes and a **Testing** section with executed commands.
