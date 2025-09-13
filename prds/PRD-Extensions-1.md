
# Veritas PRD — Verhoeff & Damm Checksum Algorithms and Unlocked Identifiers
**Project:** Veritas (identifier validation & generation)  
**Owner:** Rebecca Powell (@rebeccapowell)  
**Version:** 1.0 (2025‑09‑12)  
**Status:** Draft for implementation

---

## 1. Purpose & Goals

This PRD specifies adding two checksum algorithms (**Verhoeff** and **Damm**) to Veritas and implementing a set of identifiers that rely on these algorithms. The aim is to broaden Veritas’ coverage of globally relevant identifiers, while adhering to the project’s design constraints (allocation‑free, culture‑invariant, deterministic, testable).

### Goals
- Provide production‑grade, table‑driven implementations for Verhoeff and Damm.
- Add validators (and safe generators, where appropriate) for identifiers unlocked by these algorithms:
  1. **India Aadhaar (UID)** — 12 digits, Verhoeff check digit *(validation; test‑data generation only)*
  2. **SNOMED CT Concept ID (SCTID)** — numeric identifier with Verhoeff check digit *(validation; test‑data generation only)*
  3. **Luxembourg National ID (natural persons)** — 13 digits; digit 12 = Luhn10, digit 13 = Verhoeff *(validation; test‑data generation only)*
  4. **Singapore IPOS Application Number** — new format with Damm check *(validation; test‑data generation only)*
- Publish documentation and examples; expand unit coverage (valid/invalid/edge/transposition).

### Non‑Goals
- No online registry lookups or personal‑data resolution.
- No strong uniqueness guarantees for generators (only format‑valid **test data**).
- No breaking changes to existing namespaces/identifiers.

---

## 2. Design Principles (Veritas house style)
- **APIs:** `TryValidate(ReadOnlySpan<char> input, out <IdName>Value value)`; optionally `TryGenerate(…)` when safe.
- **Normalization:** trim, remove spaces/standard separators, uppercase if alphanumeric; retain canonical presentation in `<IdName>Value`.
- **No exceptions for control flow:** return `bool`; never throw for invalid input.
- **Allocation‑aware:** operate on spans; avoid intermediate strings.
- **Thread‑safe statics:** checksum tables stored as `static readonly`.
- **Deterministic generation:** for test data; avoid time/IO randomness by default (unless explicitly provided via options/seed).
- **Clear error semantics:** when `false`, `value = default`; reason codes provided via optional `ValidationError` (future enhancement).

---

## 3. Public API Surface

### 3.1 Checksums
```csharp
namespace Veritas.Checksums;

public static class Verhoeff
{
    // Returns value 0..9
    public static byte Compute(ReadOnlySpan<char> digits);

    // Returns true if the final digit is the correct Verhoeff check digit
    public static bool Validate(ReadOnlySpan<char> digitsWithCheck);

    // Appends the computed check digit to destination (digits + check)
    public static bool Append(ReadOnlySpan<char> digits, Span<char> destination, out int written);
}

public static class Damm
{
    public static byte Compute(ReadOnlySpan<char> digits);
    public static bool Validate(ReadOnlySpan<char> digitsWithCheck);
    public static bool Append(ReadOnlySpan<char> digits, Span<char> destination, out int written);
}
```

> **Implementation notes**
> - **Verhoeff** uses dihedral group permutations: tables `d[10,10]`, `p[8,10]`, `inv[10]`. Process right‑to‑left, starting position 1.
> - **Damm** uses a 10×10 quasigroup matrix (weakly totally anti‑symmetric). Accumulator initialized to 0, fold all digits left‑to‑right; accumulator is the check.

### 3.2 Identifiers
Each identifier follows this pattern and lives under an appropriate domain namespace:
```csharp
public static class <IdName>
{
    public static bool TryValidate(ReadOnlySpan<char> input, out <IdName>Value value);
    public static bool TryGenerate(GenerationOptions options, Span<char> destination, out int written);
}

public readonly record struct <IdName>Value(string Value)
{
    public override string ToString() => Value;
}
```

---

## 4. Identifiers — Functional Requirements

### 4.1 India Aadhaar (UID)
- **Namespace:** `Veritas.Identity.India.Aadhaar`
- **Format:** 12 digits, final digit is **Verhoeff** check.
- **Normalization:** remove spaces and common separators; require 12 digits.
- **Validation steps:**
  1. Verify length == 12 and all digits.
  2. Run `Verhoeff.Validate` on entire 12‑digit string.
  3. If valid, return normalized `AadhaarValue` (grouping not preserved).
- **Generation:** **test data only** (no PII); generate 11 random digits (optionally seeded), compute Verhoeff check, return 12 digits.
- **Edge cases:** reject inputs with all identical digits; enforce no leading zeros requirement only if documented (default: allow leading 0).
- **Security/ethics:** explicitly document non‑PII/test‑only generation.

### 4.2 SNOMED CT Concept ID (SCTID)
- **Namespace:** `Veritas.Health.Snomed.SctId`
- **Format:** numeric string; last digit is **Verhoeff** check. Overall length varies (e.g., 6–18).
- **Normalization:** remove separators; require all digits; min/max length configurable (defaults aligned to spec ranges).
- **Validation:** `Verhoeff.Validate` on full number; optional partition checks (e.g., top‑level concept ranges) **out of scope**.
- **Generation:** **test data only**; caller provides base digits or length; compute Verhoeff.
- **Edge cases:** reject empty/too short; configurable length boundaries via `GenerationOptions`.

### 4.3 Luxembourg National ID (natural persons)
- **Namespace:** `Veritas.Identity.Luxembourg.NationalId`
- **Format:** 13 digits where:
  - Digit 12 = **Luhn10** check over digits 1–11.
  - Digit 13 = **Verhoeff** check over digits 1–11.
- **Normalization:** digits only; require length 13.
- **Validation steps:**
  1. Verify digits + length.
  2. Compute Luhn10 on first 11 digits; compare with digit 12.
  3. Compute Verhoeff on first 11 digits; compare with digit 13.
  4. Return value if both checks match.
- **Generation:** **test data only**; given 11 digits (or generated), compute both checks and append.
- **Edge cases:** transposition handling: Luhn + Verhoeff combination provides robust detection; still treat as format check only.

### 4.4 Singapore IPOS Application Number (post‑2014 format)
- **Namespace:** `Veritas.IP.Singapore.IpApplicationNumber`
- **Format (canonical):** `AAYYYYNNNNNC` or `AAYYYYNNNNNC-DD`  
  - `AA`: application type (e.g., “SG” or code per scheme) – **opaque** here.  
  - `YYYY`: year.  
  - `NNNNN`: serial.  
  - `C`: **Damm** check character (may map to 0–9 or constrained alphabet per IPOS presentation).  
  - Optional `-DD`: sub‑designations (ignored for checksum).
- **Normalization:** strip spaces and hyphens; extract the core base and check; uppercase letters.
- **Validation steps:**
  1. Validate structural segments (len/charset); treat `AA` alphas as opaque.
  2. Apply **Damm** to the numeric payload defined by the scheme (commonly the numeric portion before `C`).
  3. Map computed check to allowed character set; compare to `C`.
- **Generation:** **test data only**; given `AA`, `YYYY`, and serial, compute `C` via Damm and render canonical string.
- **Edge cases:** if an alpha mapping of the Damm value is required, expose mapping in options.

> **Note:** Exact IPOS character mapping conventions vary by document/version. Provide an option model to adapt mapping without code changes.

---

## 5. Non‑Functional Requirements

- **Performance:** O(n) over digits; no heap allocations for hot paths; <1µs for short identifiers on modern CPUs.
- **Localization:** invariant culture for parsing/uppercasing.
- **Security/Privacy:** Explicit “test data only” for generators that could resemble PII; no external calls; no storage.
- **Compatibility:** Target `net8.0`. Public API additions only (no breaking changes).

---

## 6. Data Model & Options

```csharp
public readonly record struct GenerationOptions(
    int? Length = null,
    int? Seed = null,
    bool GroupForDisplay = false,
    char GroupSeparator = ' ',
    ReadOnlySpan<char> Prefix = default
);
```

- **Length:** for variable‑length identifiers (e.g., SCTID).  
- **Seed:** deterministic PRNG (e.g., xorshift32) for repeatable test data.  
- **GroupForDisplay:** render in blocks (3–4 digits) when returning `destination`.  
- **Prefix:** for schemes requiring fixed leading data (e.g., type/year).

---

## 7. Validation & Normalization Rules (Common)
- Strip whitespace and known separators (`' '`, `'-'`, `'_'`, `'/'`).  
- Uppercase ASCII letters (if present).  
- Reject non‑ASCII alphanumerics unless the scheme explicitly allows.  
- Return `false` if any rule fails; do not throw.

---

## 8. Test Plan

### 8.1 Unit Test Structure
`test/Veritas.Tests/<Domain>/<IdName>Tests.cs`  
- **Valid cases:** known good numbers (including edge/grouping variants).  
- **Invalid cases:** wrong length, bad chars, wrong checksum digit, transpositions.  
- **Normalization:** inputs with spaces/dashes/casing → normalized value equality.  
- **Transposition tests:** ensure Verhoeff/Damm catch adjacent swaps that Luhn would miss.  
- **Generation determinism:** same seed → same output; `TryValidate(TryGenerate(...)) == true`.

### 8.2 Coverage Targets
- **Statements:** ≥ 95% for new classes.  
- **Branches:** ≥ 90% (checksum tables & edge paths).

### 8.3 Example Vectors (Illustrative)
> (Replace with authoritative vectors during implementation.)
- Verhoeff: base “2363” → check “0”; “23630” valid.  
- Damm: base “572” → check “4”; “5724” valid.  
- Aadhaar: “123412341234” → must fail (demo).  
- SCTID: “24700007” (example format only) + computed check.  
- Luxembourg: construct first 11 digits; compute Luhn & Verhoeff; verify both.  
- IPOS: numeric payload → Damm value → map to check char.

---

## 9. Documentation Deliverables

- **README updates:** add algorithms to “Supported algorithms”, new IDs to “Supported identifiers”.  
- **Docs pages (DocFX):**
  - `/docs/checksums/verhoeff.md` (tables, algorithm steps, examples)
  - `/docs/checksums/damm.md`
  - `/docs/identifiers/india/aadhaar.md`
  - `/docs/identifiers/health/snomed/sctid.md`
  - `/docs/identifiers/luxembourg/national-id.md`
  - `/docs/identifiers/singapore/ip-application-number.md`
- **Change log:** `CHANGELOG.md` entry.

---

## 10. Implementation Plan

1. **Checksum primitives**
   - Add `Veritas.Checksums.Verhoeff` + tests.
   - Add `Veritas.Checksums.Damm` + tests.
2. **Identifiers**
   - Aadhaar → SCTID → Luxembourg → Singapore IP (in that order).
3. **Docs & Examples**
   - Minimal examples in README; full docs in DocFX.
4. **CI**
   - Ensure `dotnet test -f net8.0` passes.
   - (Optional) Add Cobertura coverage report and badge.

---

## 11. Acceptance Criteria

- [ ] `Verhoeff` and `Damm` classes expose `Compute`, `Validate`, `Append`, with zero allocations in the happy path.
- [ ] All four identifiers implement `TryValidate` and (when marked) `TryGenerate` for **test data**.
- [ ] Unit tests cover valid/invalid/normalization/transposition; coverage targets met.
- [ ] README and DocFX pages updated.
- [ ] CI green on `net8.0`.
- [ ] No public API breaks; analyzers show no new warnings.

---

## 12. Open Questions / Risks

- **Authoritative vectors & specs:** During implementation, verify the exact check‑digit position/length bounds and character mapping for each scheme against primary sources.
- **Singapore IPOS mapping:** Confirm whether the Damm check is rendered as a digit or mapped to an alpha set; make this configurable via `GenerationOptions` if not uniform.
- **Privacy concerns:** Generators must remain clearly **non‑PII** and intended solely for test fixtures.

---

## 13. Appendix: Pseudocode

**Verhoeff.Compute**
```
acc = 0
for i from rightmost to leftmost (index starting at 1):
  acc = d[ acc ][ p[i mod 8][ digit(i) ] ]
return inv[acc]
```

**Damm.Compute**
```
acc = 0
for each digit x in left-to-right order:
  acc = table[acc][x]
return acc
```

---

## 14. File & Namespace Layout

```
src/Veritas/Checksums/Verhoeff.cs
src/Veritas/Checksums/Damm.cs

src/Veritas/Identifiers/Identity/India/Aadhaar.cs
src/Veritas/Identifiers/Identity/India/AadhaarValue.cs

src/Veritas/Identifiers/Health/Snomed/SctId.cs
src/Veritas/Identifiers/Health/Snomed/SctIdValue.cs

src/Veritas/Identifiers/Identity/Luxembourg/NationalId.cs
src/Veritas/Identifiers/Identity/Luxembourg/NationalIdValue.cs

src/Veritas/Identifiers/IP/Singapore/IpApplicationNumber.cs
src/Veritas/Identifiers/IP/Singapore/IpApplicationNumberValue.cs

test/Veritas.Tests/Checksums/VerhoeffTests.cs
test/Veritas.Tests/Checksums/DammTests.cs
test/Veritas.Tests/Identity/India/AadhaarTests.cs
test/Veritas.Tests/Health/Snomed/SctIdTests.cs
test/Veritas.Tests/Identity/Luxembourg/NationalIdTests.cs
test/Veritas.Tests/IP/Singapore/IpApplicationNumberTests.cs
```
