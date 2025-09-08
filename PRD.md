# Veritas — Product Requirements

## Overview
- **Package ID:** `Veritas`
- **Purpose:** Normalize, validate and (where safe) generate real‑world identifiers across domains such as finance, tax, energy, identity/software, logistics & supply chain, healthcare & pharma, telecom & IT, education & research, and media & entertainment.
- **Target Frameworks:** `net8.0`; `netstandard2.0`
- **Dependencies:** none
- **Design Principles:** static types per identifier, uniform `Try*` APIs, span‑first, zero reflection, trimming/AOT friendly.

## 1. Assembly & Namespaces
- Single assembly with namespaces rooted at `Veritas`.
- **Core primitives:** `Veritas` (validation results, errors, formatting, bulk helpers, algorithms)
- **Domains:** `Veritas.Finance`, `Veritas.Tax`, `Veritas.Energy`, `Veritas.Identity`, `Veritas.Logistics`, `Veritas.Healthcare`, `Veritas.Telecom`, `Veritas.Education`, `Veritas.Media`.
- **Country sub‑namespaces:** e.g. `Veritas.Tax.DE`, `Veritas.Tax.UK`, `Veritas.Energy.DE`, `Veritas.Energy.GB`, etc.

## 2. Core Public API
```csharp
namespace Veritas;

enum ValidationError { None, Length, Charset, Checksum, CountryRule, Format, Range, ReservedPrefix }

readonly struct ValidationResult<T>
{
    public bool IsValid { get; }
    public T? Value { get; }
    public ValidationError Error { get; }
    public string? Message { get; }
    public ValidationResult(bool isValid, T? value, ValidationError error, string? message = null);
}

interface IValidator<T> { bool TryValidate(ReadOnlySpan<char> input, out ValidationResult<T> result); }
interface IGenerator<T> { bool TryGenerate(in GenerationOptions options, Span<char> destination, out int written); }

readonly struct GenerationOptions { public int Count { get; init; } public int? Seed { get; init; } }

enum FormatStyle { Compact, Grouped, Canonical }

static class Bulk
{
    static IEnumerable<string> GenerateMany(Func<Span<char>, (bool ok, int written)> tryGenerate, int count, int? seed = null);
}
```

## 3. Algorithms
Span‑based, allocation‑free implementations of:
- Luhn (mod 10)
- ISBN‑10 / ISSN weighted checksums
- VIN transliteration and weights
- ISO 7064 (mod 11,10; mod 97,10; mod 97; mod 37,2)
- Verhoeff / Damm (reserved)
- Bech32 / Bech32m and Base58Check codecs
- MRZ (ICAO 9303, weights 7‑3‑1)
- ISO 6346 container check digit
- Weighted mod 11 variants (e.g., ABA routing, MPAN)

## 4. Identifier Catalog
Legend: **V** = validate, **G** = generate (where safe).

### Finance
- IBAN — **V**, BBAN‑based generation
- BIC/SWIFT — **V**
- ISIN — **V**
- ISO 11649 RF — **V/G**
- Payment card PAN — **V**, test number **G**
- ABA routing — **V**
- Mexican CLABE — **V/G**
- Legal Entity Identifier (LEI) — **V/G**
- SEDOL — **V/G**
- CUSIP — **V/G**
- Market Identifier Code (MIC) — **V**
- German WKN — **V**

### Tax
- DE: USt‑IdNr, IdNr
- UK: UTR, VAT, NINO, CompanyNumber
- FR: Siren, Siret, VAT
- IT: PIVA
- ES: NIF, NIE, CIF
- NL: BSN, BTW
- PL: NIP, REGON, PESEL
- SE: Personnummer, OrgNr
- US: SSN — V/G, EIN, ITIN
- BR: CPF, CNPJ
- CA: SIN, BN
- IN: PAN, GSTIN
- AU: TFN, ABN
- CN: USCC
- NZ: IRD

### Energy
- EIC
- DE: MaLo, MeLo, ZPN
- GB: MPAN, MPRN
- FR: PRM
- NL: Energy EAN
- IT: POD, PDR
- ES: CUPS

### Identity & Software
- ULID, UUID, NanoID, KSUID
- Email address
- Phone number (E.164)
- Domain name
- BCP‑47 language tag
- Ethereum address
- Base58Check

### Logistics & Supply Chain
- GTIN/EAN/UPC
- Global Location Number (GLN)
- Serial Shipping Container Code (SSCC)
- Vehicle Identification Number (VIN)
- ISO 6346 container code
- Air Waybill (AWB)
- IMO ship identification number

### Healthcare & Pharma
- NHS Number — V/G
- ORCID — V/G

### Telecom & IT
- IMEI
- MEID
- ICCID
- MAC — V/G
- OUI
- ASN
- IPv4 / IPv6

### Education & Research
- DOI
- ISNI
- ISMN

### Media & Entertainment
- ISBN‑10 / ISBN‑13
- ISSN
- ISRC

## 5. Testing
- Golden vectors per identifier (official examples where available)
- Property/fuzz testing for length, charset and checksum failures
- Concurrency and culture tests
- Trim/AOT sample app

## 6. Documentation & Developer Experience
- XML documentation for public APIs, SourceLink and symbol packages
- README with quick‑start, support matrix and code samples
- Sample console demonstrating common tasks (IBAN, GTIN, IMEI, MPAN, CPF, ORCID, MRZ)

## 7. Example Usage
```csharp
// Finance: IBAN
if (Finance.Iban.TryValidate("FR14 2004 1010 0505 0001 3M02 606", out var iban) && iban.IsValid)
    Console.WriteLine(iban.Value!.Value); // compact normalized

// Logistics: generate a GTIN‑13
foreach (var s in Bulk.GenerateMany((dst, rng) => {
    var ok = Logistics.Gtin.TryGenerate(13, new GenerationOptions { Seed = rng.Next() }, dst, out var w);
    return (ok, w);
}, 5, seed: 42))
    Console.WriteLine(s);

// Telecom: IMEI
Telecom.Imei.TryValidate("490154203237518", out var imei);
```
