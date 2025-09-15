# Veritas PRD — Crypto Address Checksum Algorithms and Identifiers
**Project:** Veritas (identifier validation & generation)
**Owner:** Rebecca Powell (@rebeccapowell)
**Version:** 1.0 (2025-09-19)
**Status:** Draft for implementation

---

## 1. Purpose & Goals

This PRD proposes adding checksum algorithms used in major cryptocurrency and blockchain identifiers, and implementing a set of validators/generators for those identifiers. The goal is to extend Veritas beyond traditional finance and government IDs into the digital asset domain while staying true to Veritas' design constraints (allocation-free, deterministic, culture-invariant).

### Goals
- Implement **Base58Check** (double SHA-256) and **Bech32/Bech32m** (BIP-173/BIP-350 polymod) codecs.
- Implement **EIP-55** mixed-case checksum (Keccak-256) for Ethereum addresses.
- Add validators (and safe generators, where appropriate) for identifiers unlocked by these algorithms:
  1. **Bitcoin Legacy Address** — Base58Check (validation; test-data generation only).
  2. **Bitcoin SegWit Address** — Bech32/Bech32m (validation; test-data generation only).
  3. **Lightning Network Invoice (BOLT11)** — Bech32 (validation only).
  4. **Ethereum Address** — EIP-55 (validation; test-data generation only).
- Publish documentation and examples; expand unit coverage (valid/invalid/edge cases).

### Non-Goals
- No key or signature derivation.
- No network lookups or balance queries.
- No support for chains beyond those explicitly listed.
- No guarantee of uniqueness beyond checksum validity (generators produce **test addresses** only).

---

## 2. Design Principles (Veritas house style)
- **APIs:** `TryValidate(ReadOnlySpan<char> input, out <IdName>Value value)`; optional `TryGenerate(...)` when safe.
- **Normalization:** trim, remove common separators, lowercase for hex/base encodings unless checksum requires case.
- **No exceptions for control flow:** return `bool`; invalid input never throws.
- **Allocation-aware:** operate on spans; avoid intermediate strings.
- **Thread-safe statics:** checksum constants/tables stored as `static readonly`.
- **Deterministic generation:** for test data; seedable PRNG.
- **Clear error semantics:** on failure, `value = default` and caller receives `false`.

---

## 3. Public API Surface

### 3.1 Checksums & Codecs
```csharp
namespace Veritas.Checksums;

public static class Base58Check
{
    public static bool TryDecode(ReadOnlySpan<char> input, Span<byte> destination, out int bytesWritten);
    public static bool TryEncode(ReadOnlySpan<byte> payload, Span<char> destination, out int charsWritten);
    public static bool Validate(ReadOnlySpan<char> input);
}

public static class Bech32
{
    public static bool TryDecode(ReadOnlySpan<char> input, Span<byte> data, out int dataLen, out string hrp, out bool bech32m);
    public static bool TryEncode(ReadOnlySpan<byte> data, ReadOnlySpan<char> hrp, bool bech32m, Span<char> destination, out int written);
    public static bool Validate(ReadOnlySpan<char> input);
}

public static class Eip55
{
    public static bool Validate(ReadOnlySpan<char> hexAddress);
    public static bool TryChecksum(ReadOnlySpan<char> hexAddress, Span<char> destination, out int written);
}
```

> **Implementation notes**
> - **Base58Check**: decode/encode using Bitcoin alphabet; checksum = last 4 bytes of double SHA-256 of version+payload.
> - **Bech32/Bech32m**: implement polymod with generator `0x3b6a57b2`; handle HRP expansion and variant constant (1 for bech32, 0x2bc830a3 for bech32m).
> - **EIP-55**: Keccak-256 hash of lowercase hex address; use hash bits to determine case of each hex digit.

### 3.2 Identifiers
Pattern for each identifier:
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

### 4.1 Bitcoin Legacy Address
- **Namespace:** `Veritas.Crypto.Bitcoin.LegacyAddress`
- **Format:** Base58Check string representing `version(1 byte) + payload(20 bytes) + checksum(4 bytes)`.
- **Normalization:** strip leading/trailing whitespace.
- **Validation:**
  1. Decode Base58.
  2. Verify length 25 bytes.
  3. Compute double SHA-256 of first 21 bytes; compare first 4 bytes to checksum.
  4. Return normalized `LegacyAddressValue` with original casing.
- **Generation:** **test data only**; given version byte and 20-byte payload (random/seeded), compute checksum and encode.
- **Edge cases:** reject non-Bitcoin alphabets; disallow leading zero unless version dictates.

### 4.2 Bitcoin SegWit Address
- **Namespace:** `Veritas.Crypto.Bitcoin.SegWitAddress`
- **Format:** `hrp("bc" or "tb") + '1' + data + 6-char checksum` using Bech32 or Bech32m depending on witness version.
- **Normalization:** lowercase; strip spaces.
- **Validation:**
  1. Ensure HRP is known (`bc`, `tb`).
  2. `Bech32.Validate` and extract data + witness version.
  3. If witness version ≥ 1 → require Bech32m checksum; otherwise Bech32.
  4. Return value containing canonical address.
- **Generation:** **test data only**; given HRP, witness version and program, encode using appropriate checksum variant.
- **Edge cases:** enforce witness program length (2–40 bytes) and version range (0–16).

### 4.3 Lightning Network Invoice (BOLT11)
- **Namespace:** `Veritas.Crypto.Lightning.Invoice`
- **Format:** `ln` + currency HRP + optional amount + `1` + Bech32 data + 6-char checksum.
- **Normalization:** lowercase; strip spaces.
- **Validation:**
  1. Split HRP and data; validate currency prefix (e.g., `bc`, `tb`).
  2. `Bech32.Validate` entire string (always Bech32, not Bech32m).
  3. Decode minimal fields (timestamp & signature) to ensure structure.
  4. Return normalized value if checksum and structure are valid.
- **Generation:** *none* (out of scope for this PRD).
- **Edge cases:** ignore optional amount and tagged fields beyond structural check.

### 4.4 Ethereum Address
- **Namespace:** `Veritas.Crypto.Ethereum.Address`
- **Format:** `0x` optional prefix followed by 40 hex chars; mixed-case per EIP-55.
- **Normalization:** remove `0x` if present; lowercase before checksum.
- **Validation:**
  1. Verify length == 40 hex characters.
  2. Run `Eip55.Validate` (Keccak-256 based case check).
  3. Return canonical `AddressValue` with `0x` prefix and checksummed casing.
- **Generation:** **test data only**; given 20-byte payload (random/seeded), compute checksum casing.
- **Edge cases:** allow all-zero address only if explicitly requested; treat non-hex chars as invalid.

---

## 5. Non-Functional Requirements
- **Performance:** O(n) over input size; no heap allocations in hot paths; <1µs for typical lengths on modern CPUs.
- **Localization:** invariant culture for hex/base conversions.
- **Security/Privacy:** generators produce deterministic **test addresses**; emphasize non-production usage.
- **Compatibility:** Target `net8.0`; no additional dependencies.

---

## 6. Data Model & Options

```csharp
public readonly record struct GenerationOptions(
    ReadOnlySpan<byte> Prefix = default,
    int? Seed = null,
    bool IncludePrefix = true
);
```
- **Prefix:** version byte (Bitcoin) or HRP (SegWit) supplied by caller.
- **Seed:** deterministic PRNG for repeatable test data.
- **IncludePrefix:** whether to include `0x` in Ethereum output or HRP in Bech32.

---

## 7. Validation & Normalization Rules (Common)
- Strip whitespace and common separators (`' '`, `'-'`).
- Lowercase for Bech32 and Ethereum prior to checksum evaluation (EIP-55 reapplies case).
- Reject non-ASCII alphanumerics.
- Return `false` for any rule failure; never throw.

---

## 8. Test Plan

### 8.1 Unit Test Structure
`test/Veritas.Tests/<Domain>/<IdName>Tests.cs`
- **Valid cases:** official vectors from BIPs/EIPs.
- **Invalid cases:** bad checksum, wrong HRP, invalid length, illegal characters.
- **Normalization:** inputs with spacing/casing → normalized value equality.
- **Generation determinism:** same seed → same output (where generation supported).

### 8.2 Coverage Targets
- **Statements:** ≥ 95% for new classes.
- **Branches:** ≥ 90% (hash edge cases, witness version switches).

### 8.3 Example Vectors (Illustrative)
> (Replace with authoritative vectors during implementation.)
- Base58Check: `1BoatSLRHtKNngkdXEeobR76b53LETtpyT` valid.
- SegWit: `bc1qw508d6qejxtdg4y5r3zarvary0c5xw7k7gtv0w` valid (Bech32).
- Lightning Invoice: `lnbc2500u1pwxx...` (truncated).
- Ethereum: `0x52908400098527886E0F7030069857D2E4169EE7` valid EIP-55.

---

## 9. Documentation Deliverables
- **README updates:** add algorithms to “Supported algorithms” and identifiers to “Supported identifiers”.
- **Docs pages (DocFX):**
  - `/docs/checksums/base58check.md`
  - `/docs/checksums/bech32.md`
  - `/docs/checksums/eip55.md`
  - `/docs/identifiers/crypto/bitcoin/legacy-address.md`
  - `/docs/identifiers/crypto/bitcoin/segwit-address.md`
  - `/docs/identifiers/crypto/lightning/invoice.md`
  - `/docs/identifiers/crypto/ethereum/address.md`
- **Change log:** `CHANGELOG.md` entry.

---

## 10. Implementation Plan
1. **Checksum primitives**
   - Add `Veritas.Checksums.Base58Check` + tests.
   - Add `Veritas.Checksums.Bech32` + tests.
   - Add `Veritas.Checksums.Eip55` (Keccak-256) + tests.
2. **Identifiers**
   - Bitcoin Legacy → SegWit → Lightning Invoice → Ethereum Address (in that order).
3. **Docs & Examples**
   - Minimal examples in README; full docs in DocFX.
4. **CI**
   - Ensure `dotnet test -f net8.0` passes.

---

## 11. Acceptance Criteria
- [ ] Base58Check, Bech32/Bech32m, and EIP-55 classes expose `Validate` and encode/decode helpers with zero allocations.
- [ ] All four identifiers implement `TryValidate` and, where marked, `TryGenerate` for **test data**.
- [ ] Unit tests cover valid/invalid/normalization cases; coverage targets met.
- [ ] README and DocFX pages updated.
- [ ] CI green on `net8.0`.
- [ ] No public API breaks; analyzers show no new warnings.

---

## 12. Open Questions / Risks
- **Keccak-256 implementation:** confirm license and performance; consider simplified Keccak for small inputs.
- **Lightning Invoice scope:** how much of the invoice should be parsed vs treated as opaque payload.
- **Bech32 HRP list:** whether to hardcode known HRPs or allow any and defer to caller.
- **Security disclaimer:** emphasize that generated addresses are not backed by private keys.

---

## 13. File & Namespace Layout
```
src/Veritas/Checksums/Base58Check.cs
src/Veritas/Checksums/Bech32.cs
src/Veritas/Checksums/Eip55.cs

src/Veritas/Identifiers/Crypto/Bitcoin/LegacyAddress.cs
src/Veritas/Identifiers/Crypto/Bitcoin/LegacyAddressValue.cs

src/Veritas/Identifiers/Crypto/Bitcoin/SegWitAddress.cs
src/Veritas/Identifiers/Crypto/Bitcoin/SegWitAddressValue.cs

src/Veritas/Identifiers/Crypto/Lightning/Invoice.cs
src/Veritas/Identifiers/Crypto/Lightning/InvoiceValue.cs

src/Veritas/Identifiers/Crypto/Ethereum/Address.cs
src/Veritas/Identifiers/Crypto/Ethereum/AddressValue.cs

test/Veritas.Tests/Checksums/Base58CheckTests.cs
test/Veritas.Tests/Checksums/Bech32Tests.cs
test/Veritas.Tests/Checksums/Eip55Tests.cs

test/Veritas.Tests/Crypto/Bitcoin/LegacyAddressTests.cs
test/Veritas.Tests/Crypto/Bitcoin/SegWitAddressTests.cs
test/Veritas.Tests/Crypto/Lightning/InvoiceTests.cs
test/Veritas.Tests/Crypto/Ethereum/AddressTests.cs
```
