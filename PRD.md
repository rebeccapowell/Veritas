Veritas — Complete Specification (v1)

Package ID: Veritas (single NuGet)
Purpose: Normalize, validate, and (where safe) generate identifiers across Finance, Tax, Energy, Identity/Software, plus Logistics & Supply Chain, Healthcare & Pharma, Telecom & IT, Education & Research, Media & Entertainment, Government & Legal, and Retail & Commerce.
TFMs: net8.0;netstandard2.0
Deps: none
Design: static types per ID, uniform Try* APIs, Span-first, zero reflection, trimming/AOT-friendly.

⸻

1) Assembly & Namespaces

Single assembly. Logical namespaces (all under Veritas):

Core primitives:
  Veritas                // ValidationResult, errors, formatting, bulk, algorithms

Domains:
  Veritas.Finance
  Veritas.Tax
  Veritas.Energy
  Veritas.Identity
  Veritas.Logistics
  Veritas.Healthcare
  Veritas.Telecom
  Veritas.Education
  Veritas.Media
  Veritas.Gov
  Veritas.Retail

Country-specific subnamespaces where relevant:
  Veritas.Tax.DE, .UK, .FR, .IT, .ES, .NL, .PL, .SE, .EU, .US, .BR, .CA, .IN, .AU, .CN, .NZ
  Veritas.Energy.DE, .GB, .FR, .NL, .IT, .ES


⸻

2) Core Public API (uniform)

namespace Veritas;

public enum ValidationError { None, Length, Charset, Checksum, CountryRule, Format, Range, ReservedPrefix }

public readonly struct ValidationResult<T>
{
    public bool IsValid { get; }
    public T? Value { get; }                 // normalized strong type; null when invalid
    public ValidationError Error { get; }    // reason if invalid
    public string? Message { get; }          // optional (EN in v1)
    public ValidationResult(bool isValid, T? value, ValidationError error, string? message = null);
}

public interface IValidator<T>
{
    bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<T> result);
}

public interface IGenerator<T>
{
    bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written);
}

public readonly struct GenerationOptions
{
    public int Count { get; init; }                // used by Bulk
    public int? Seed { get; init; }                // deterministic generation
    // optional per-ID fields realized via overloads / helpers
}

public enum FormatStyle { Compact, Grouped, Canonical }

public static class Bulk
{
    public static IEnumerable<string> GenerateMany(
        Func<Span<char>, (bool ok, int written)> tryGenerate,
        int count,
        int? seed = null);
}

Per-ID conventions
	•	static class {IdName} with:
	•	TryValidate(ReadOnlySpan<char> s, out ValidationResult<{IdName}Value> r)
	•	TryFormat(ReadOnlySpan<char> s, FormatStyle style, Span<char> dest, out int written) (if meaningful)
	•	Normalize(ReadOnlySpan<char> s, Span<char> dest, out int written) (strip separators/whitespace; uppercase if spec requires)
	•	TryGenerate(..., Span<char> dest, out int written) (only where safe and spec-defined)
	•	readonly struct {IdName}Value { public string Value { get; } /* + metadata if useful */ }

⸻

3) Algorithms (internal/shared)
	•	Luhn (mod 10) — GTIN/UPC/ISBN-13/GLN/SSCC/IMEI/ICCID/etc.
	•	ISBN-10/ISSN weighted checksums
	•	VIN transliteration + weights
	•	ISO 7064 (mod 11,10; mod 97,10; mod 97; mod 37,2) — IBAN/ISIN/RF/EIC/various tax IDs
	•	Verhoeff / Damm (reserved for IDs that use them)
	•	Bech32/Bech32m + Base58Check enc/dec
	•	MRZ (ICAO 9303) check-digit (weights: 7-3-1 pattern)
	•	ISO 6346 container check-digit
	•	Mod 11 weighted variants (ABA routing, MPAN, many national IDs)

All Span-based, allocation-free.

⸻

4) Identifier Catalog (Validate / Generate)

Legend: V = Validate, G = Generate (syntactic/test-safe).
Where only format exists (no checksum), implement fast structural validation (manual scan or compiled regex on netstandard2.0).

4.1 Finance
	•	IBAN — V, TryFromBban(country, bban) G; TryFormat (grouped/compact)
	•	BIC/SWIFT — V (8/11 chars, structure)
	•	ISIN — V (mod 97)
	•	ISO 11649 RF — V/G (mod 97, “RF” + 2 digits + up to 21 alnum)
	•	Payment Card PAN — V (Luhn, 12–19); G (test numbers only, brand-agnostic)
	•	US ABA Routing — V (weighted mod 10)
        •       Mexican CLABE — V/G (weighted mod 10)
        •       Legal Entity Identifier (LEI) — V/G (mod 97)

4.2 Tax (country-specific; validate-only unless spec allows test gen)

DE: USt-IdNr, IdNr
UK: UTR, VAT, NINO, Company No.
FR: SIREN/SIRET, VAT
IT: Codice Fiscale, Partita IVA
ES: NIF, NIE, CIF
NL: BSN, BTW
PL: NIP, REGON, PESEL
SE: Personnummer, OrgNr
US: SSN (structural + disallow invalid ranges), EIN, ITIN
BR: CPF, CNPJ
CA: SIN, BN
IN: PAN, GSTIN
AU: TFN, ABN
CN: USCC
NZ: IRD

(Most use ISO 7064 or mod 11-style checks. Provide format-only where that’s all that exists.)

4.3 Energy

Global
	•	EIC — V/G (checksum)

Country
	•	DE: MaLo (11 digits) V/G, MeLo (33 chars) V/G, Zählpunktnummer V
	•	GB: MPAN (21 digits, weighted check) V/G, MPRN (6–10 digits) V
	•	FR: PRM (14 digits) V
	•	NL: Energy EAN (18 digits, GS1 mod 10) V/G
	•	IT: POD (IT + 14 alnum + checksum) V, PDR (14 digits) V
	•	ES: CUPS (20–22 chars incl. 2 check letters) V/G

4.4 Identity / Software
	•	ULID — V/G (Crockford base32; sortable)
	•	KSUID — V/G
	•	NanoID — V/G (configurable alphabet/length)
	•	UUID/GUID — V (v1–v5 parsing); G (v4; optional v3/v5)
	•	Email (pragmatic) — V (no DNS)
	•	Phone (E.164-lite) — V (+ + 8..15 digits)
	•	Domain/Hostname (LDH) — V
	•	BCP-47 language tag — V
	•	Bech32/Bech32m, Base58Check — V/G
	•	Ethereum (EIP-55) — V (checksum-case); G (test vectors only)

4.5 Logistics & Supply Chain
	•	GTIN-12/13/14, EAN-8, UPC-A/E — V/G (Luhn)
	•	GLN, SSCC-18 — V/G (Luhn)
	•	VIN — V (transliteration + weights; check digit at pos 9)
	•	ISO 6346 Container Number — V (owner code + category + serial + check)
	•	IATA Air Waybill (AWB) — V (3-digit prefix + 8-digit serial + check)
	•	IMO ship number — V (7 digits with check)

4.6 Healthcare & Pharma
	•	NHS Number (UK) — V (10 digits, mod 11 weights)
	•	US NDC — V (10/11 digits; structural + zero-expanded 11-digit format)
	•	ISBT-128 — V (structure; checksum where applicable in components)
	•	ORCID — V (16-digit with ISO 7064 mod 11-2; display with hyphens)
	•	(Optional later: ICD/SNOMED structure checks)

4.7 Telecom & IT
	•	IMEI — V/G (15-digit with Luhn; TAC + serial + check)
	•	MEID — V (hex 14 chars; with decimal representation; checksum rules vary)
	•	ICCID (SIM) — V/G (up to 22 digits; Luhn)
	•	MAC Address — V (48-bit hex with :/-/no sep; case-insensitive), Format variants
	•	OUI — V (first 24 bits of MAC; structural)
	•	ASN (Autonomous System Number) — V (0–4294967295; disallow reserved ranges optionally)
	•	IPv4/IPv6 — V (syntax; CIDR optional)

4.8 Education & Research
	•	ORCID — (listed above; also under Education)
	•	DOI — V (syntax per prefix/suffix; not resolving)
	•	ISNI — V (16-char with check)
	•	ISMN — V (music; old/new formats with check)

4.9 Media & Entertainment
	•	ISBN-10/13 — V/G
	•	ISSN — V/G
	•	ISAN — V (structure + check)
	•	ISRC — V (12 chars; structure)
	•	ISWC — V (starts with “T”, digits + check)

4.10 Government & Legal
	•	Passport MRZ (ICAO 9303) — V (check digits over fields; MRZ line structure)
	•	National ID variants (beyond Tax set): e.g., ES DNI V/G (letter check), CZ Rodné číslo V (date/format + mod 11)
	•	Driver’s Licenses — (many formats; start with structural validators for large markets)
	•	HS/KN customs codes — V (numeric length, ranges)

4.11 Retail & Commerce
	•	Gift Card/Loyalty IDs — V/G where Luhn/weights known (generic Luhn profiles)
	•	Coupon/Voucher — (optional tie-in) keep separate or provide light helpers

⸻

5) Public Type Shape (samples)

// Logistics – ISO 6346 Container
public static class Iso6346
{
    public static bool TryValidate(ReadOnlySpan<char> s, out ValidationResult<Iso6346Value> r);
    public static bool Normalize(ReadOnlySpan<char> s, Span<char> dest, out int written);
}
public readonly struct Iso6346Value { public string Value { get; } public string OwnerCode { get; } public char Category { get; } }

// Telecom – IMEI
public static class Imei
{
    public static bool TryValidate(ReadOnlySpan<char> s, out ValidationResult<ImeiValue> r);
    public static bool TryGenerate(ReadOnlySpan<char> tac /* 8 digits */, Span<char> dest, out int written);
}

// Healthcare – NHS Number
public static class NhsNumber
{
    public static bool TryValidate(ReadOnlySpan<char> s, out ValidationResult<NhsNumberValue> r);
    public static bool TryFormat(ReadOnlySpan<char> s, FormatStyle style, Span<char> dest, out int written); // e.g., 3-3-4 grouping
}


⸻

6) Normalization, Formatting, Regex-only
	•	Normalization: strip spaces, hyphens, underscores; uppercase where required; do not convert homoglyphs by default (optional toggle later).
	•	Formatting: ID-specific grouping (IBAN 4-tuples; ISBN hyphenation limited to simple variants unless full ranges implemented).
	•	Regex-only IDs: Use compiled regex on netstandard2.0 if needed; prefer manual scans on net8.0 for speed. Maintain same API.

⸻

7) Generation (single + bulk)

Generate when:
	•	The spec defines a checksum (Luhn/ISO 7064/other) and generation can produce syntactically valid test values without implying real, assigned resources.
	•	Examples: GTIN/EAN/UPC, GLN, SSCC, ISBN, ISSN, IMEI, ICCID, RF, EIC, MPAN (check digit), ISIN (optional), ES DNI letter.

Bulk helper (memory-stable, streaming):

foreach (var s in Bulk.GenerateMany(
           tryGenerate: dst => { var ok = Gtin13.TryGenerate(prefix, dst, out var w); return (ok, w); },
           count: 10_000,
           seed: 123))
{
    // write out strings
}


⸻

8) Performance Targets (net8 Release)
	•	Simple checksums (Luhn, mod 11/97): ≤ 100 ns/op, 0 allocs
	•	IBAN: ≤ 200 ns/op, 0 allocs
	•	VIN/ISO 6346/MRZ: ≤ 600–1200 ns/op, 0 allocs
	•	ULID generate: ≤ 120 ns/op, 0 allocs
	•	Regex-only (Email/Domain): ≤ 1 µs avg

⸻

9) Data & Rules
	•	Rules in code (readonly arrays/tables, switch); no runtime JSON.
	•	IBAN per-country length table; VIN transliteration/weights; MRZ weights; ISO 6346 weights; VAT/company rules.
	•	Keep comments with authoritative references (spec names/numbers) for each table.

⸻

10) Project Layout

/src/Veritas/
  Veritas.csproj
  Core/
    ValidationError.cs
    ValidationResult.cs
    Formatting.cs
    Bulk.cs
    Algorithms/
      Luhn.cs
      Iso7064.cs
      VinMap.cs
      Mrz.cs
      Iso6346.cs
      Bech32.cs
      Base58Check.cs
  Finance/ (Iban, Bic, Isin, Rf, Pan, AbaRouting)
  Tax/
    EU/ (Vat)
    DE/ (UstIdNr, IdNr)
    UK/ (Utr, Vat, Nino, CompanyNumber)
    FR/ (Siren, Siret, Vat)
    IT/ (CodiceFiscale, Piva)
    ES/ (Nif, Nie, Cif)
    NL/ (Bsn, Btw)
    PL/ (Nip, Regon, Pesel)
    SE/ (Personnummer, OrgNr)
    US/ (Ssn, Ein, Itin)
    BR/ (Cpf, Cnpj)
    CA/ (Sin, Bn)
    IN/ (Pan, Gstin)
    AU/ (Tfn, Abn)
    CN/ (Uscc)
    NZ/ (Ird)
  Energy/
    Eic.cs
    DE/ (Malo, Melo, Zpn)
    GB/ (Mpan, Mprn)
    FR/ (Prm)
    NL/ (EnergyEan)
    IT/ (Pod, Pdr)
    ES/ (Cups)
  Identity/ (Ulid, Ksuid, NanoId, Uuid, Email, Phone, Domain, Bcp47, Bech32, Base58Check, Ethereum)
  Logistics/ (Gtin*, Upc*, Ean8, Gln, Sscc, Vin, Iso6346, Awb, Imo)
  Healthcare/ (NhsNumber, Ndc, Isbt128, Orcid)
  Telecom/ (Imei, Meid, Iccid, Mac, Oui, Asn, Ipv4, Ipv6)
  Education/ (Doi, Isni, Ismn)
  Media/ (Isbn*, Issn, Isan, Isrc, Iswc)
/test/...
/bench/...


⸻

11) Testing
	•	Golden vectors per ID (official examples where public).
	•	Property/Fuzz: lengths, charset, checksum failures.
	•	Concurrency: many-thread generation/validation.
	•	Culture: run under several cultures.
	•	Trim/AOT: sample app validates/generates common IDs with trimming enabled.

⸻

12) Documentation & DX
	•	XML docs for public APIs; SourceLink; symbols (snupkg).
	•	README: Quickstart, per-domain tables (V/G), code snippets, performance notes.
	•	Samples: minimal console demonstrating common tasks (IBAN, GTIN, IMEI, MPAN, CPF, ORCID, MRZ).

⸻

13) Acceptance Criteria (v1.0)
	•	Builds for net8.0;netstandard2.0, zero dependencies, no warnings.
	•	Implements the bold subset below with tests & docs:

Finance: IBAN, BIC, RF, ISIN, PAN (validate), ABA
Tax: DE (USt-IdNr, IdNr), UK (UTR, VAT, NINO, Company), FR (SIREN/SIRET, VAT), IT (PIVA), ES (NIF/NIE/CIF), NL (BSN/BTW), PL (NIP/REGON/PESEL), SE (Personnummer/OrgNr), EU VAT, US (SSN/EIN/ITIN), BR (CPF/CNPJ)
Energy: EIC; DE (MaLo/MeLo), GB (MPAN/MPRN), NL (Energy EAN), ES (CUPS), FR (PRM)
Identity: ULID, UUID, NanoID, KSUID, Email, Phone, Domain, BCP-47, Ethereum address, Base58Check
Logistics: GTIN/EAN/UPC, GLN, SSCC, VIN, ISO 6346, AWB, IMO
Healthcare: NHS Number, ORCID
Telecom: IMEI, ICCID, MAC, IPv4/IPv6
Education/Media: ISBN-10/13, ISSN, DOI, ISNI, ISMN, ISRC
Generation implemented for: RF, MPAN core, Energy EAN, CUPS, GTIN, GLN, SSCC, IMEI, ICCID, AWB, IMO, ISNI, ISMN, ISBN-10/13, ISSN, ULID, NanoID, KSUID, CPF, CNPJ, USt-IdNr, IdNr
	•	Performance targets met (see §8).
	•	Tests ≥ 90% coverage for algorithms & critical validators.
	•	README includes a support matrix (V/G) and examples.

⸻

14) Example Usage

// Finance: IBAN
if (Finance.Iban.TryValidate("FR14 2004 1010 0505 0001 3M02 606", out var iban) && iban.IsValid)
    Console.WriteLine(iban.Value!.Value); // compact normalized

// Logistics: generate GTIN-13 with known 7-digit prefix
var prefix = "4006381".AsSpan();
foreach (var s in Bulk.GenerateMany(
         dst => { var ok = Logistics.Gtin13.TryGenerate(prefix, dst, out var w); return (ok, w); },
         count: 1000, seed: 42))
{
    Console.WriteLine(s);
}

// Energy: MPAN (GB)
Energy.GB.Mpan.TryValidate("101234567890123456789", out var mpan);

// Tax: Brazil CPF
Tax.BR.Cpf.TryValidate("111.444.777-35", out var cpf);

// Telecom: IMEI
Telecom.Imei.TryValidate("490154203237518", out var imei);

// Gov: Passport MRZ line checks
Gov.PassportMrz.TryValidate(line1.AsSpan(), line2.AsSpan(), out var mrzResult);


⸻

Include extensive unit tests in a well structures manner.
include the GitHub Actions workflows and YAML for CI/CD with NuGet publication and vweaon releaae managwment with automarix release notes.
Include readme and keep it ip to date
Include an auto generated redthedocs style output
Include a aeparate clean well structured CLI tool
