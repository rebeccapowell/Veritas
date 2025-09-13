# PRD-Extensions-3 --- Additional Identifiers (Gaps vs Current Implementation)

**Goal.** Extend Veritas with widely used identifiers that have
well-defined validation rules (often check digits), complementing the
sets already shipped in Finance, Logistics, Healthcare, Customs, and
Telecom.

**Non-Goals.** Assignment/verification against live registries (e.g.,
VIES/EORI services). These are format/checksum validators and safe
test-data generators only.

## At-a-glance table

  ---------------------------------------------------------------------------------------------
  sector         country code   identifier        validation strategy -- generation strategy
                                                  algorithm              
  -------------- -------------- ----------------- ---------------------- ----------------------
  Finance        EU (multi)     **SEPA Creditor   Structure + **ISO 7064 Build string from
                                Identifier        Mod 97-10** check      country rules
                                (SCI)**           digits (positions      (incl. optional
                                                  3--4) per EPC          business code),
                                                  overview.              compute Mod 97 check
                                                                         digits; emit
                                                                         normalized form.

  Finance        FR             **RIB key**       **Mod 97** key over    Accept components,
                                (domestic account bank+branch+account;   compute clé (98 − (num
                                control key)      verify 2-digit clé     % 97)), append; sample
                                                  RIB.                   generator makes
                                                                         plausible bank/branch
                                                                         ranges.

  Finance        ES             **CCC** (Código   Dual control digits    Generate
                                Cuenta Cliente)   via **weighted Mod     bank/branch/account,
                                                  11** (bank/branch and  compute two control
                                                  account parts).        digits, assemble
                                                                         20-digit CCC (and
                                                                         optionally map into
                                                                         IBAN).

  Logistics      --- (global)   **UPU S10**       Structure              Generate 8-digit
                                postal tracking   AA#########BB +        serial, compute check
                                                  **weighted Mod 11**    digit per S10, add
                                                  (weights               service & country
                                                  8,6,4,2,3,5,9,7) with  codes; support
                                                  special 10→0, 11→5.    realistic service
                                                                         codes.

  Healthcare     US             **NPI** (National **Luhn** after         Generate 9 random
                                Provider          prefixing **80840** to digits, compute Luhn
                                Identifier)       the 9-digit base;      on 80840+base to get
                                                  verify last digit.     check digit; format as
                                                                         10 digits.

  Healthcare /   US             **DEA Number**    Structure (2 letters + Choose valid prefix
  Regulatory                                      7 digits); checksum:   letter(s) and
                                                  (d1+d3+d5) +           registrant initial,
                                                  2×(d2+d4+d6); last     generate 6 digits,
                                                  digit == d7.           compute the 7th; emit
                                                                         both classic and "9"
                                                                         business variant.

  Customs / Tax  EU / GB        **EORI**          Structure: **CC** + up Generate structurally
                                (Economic         to 15 alphanumerics;   valid test IDs
                                Operator          some MS derive from    (country prefix +
                                Registration &    VAT (country-specific  numeric core). If MS
                                Identification)   checks). Baseline =    rules known, compute
                                                  structural validation, national check(s);
                                                  optional per-MS        otherwise mark as
                                                  checksum hooks.        structural-only.

  Telecom        --- (global)   **IMSI** (Mobile  Structural validation: Generate from MCC/MNC
                                Subscriber        MCC (3) + MNC (2--3) + tables (extensible
                                Identity)         MSIN (rest); length    registry), fill MSIN
                                                  15. No checksum.       with random digits to
                                                                         length 15; ensure
                                                                         leading zeros
                                                                         preserved.

  Identity /     --- (global)   **ICAO 9303 MRZ   Full MRZ line          Construct field groups
  Travel                        Lines** (TD1/TD3) checksum(s) using      per document type
                                                  **7-3-1** weights      (passport/ID), compute
                                                  across fields (you     each check digit;
                                                  already have the MRZ   provide sample
                                                  checksum primitive;    realistic generators
                                                  this wires the full    (names, dates,
                                                  field set).            expiries).

  Logistics /    --- (global)   **GDTI** (GS1     GS1 structure + **GS1  Generate body
  GS1                           Document Type     Mod 10** check digit   (including document
                                Identifier)       (similar to GTIN).     issuer), compute Mod
                                                                         10, output
                                                                         with/without AI (253)
                                                                         representation.

  Logistics /    --- (global)   **GIAI** (Global  GS1 structure; no      Generate per GS1
  GS1                           Individual Asset  check digit in pure    character rules;
                                Identifier)       element string;        optionally
                                                  structural             numeric-only variant
                                                  validation + character for Mod 10 demo data.
                                                  set rules; optional    
                                                  Mod 10 on numeric      
                                                  bodies if used by      
                                                  implementers.          
  ---------------------------------------------------------------------------------------------

> Notes:\
> • "Country code" is **ISO 3166-1 alpha-2** where applicable.\
> • Where national variations exist (e.g., EORI derived from VAT or
> country-specific pre-checks), the proposal is to implement a
> **baseline** validator plus **pluggable country rules** so you can
> expand safely without breaking changes.

## Rationale & fit

-   **Finance (SCI, RIB, CCC).** Developers often need domestic account
    checks during legacy migrations or when IBAN is not yet available;
    these are mature, checksum-backed formats.\
-   **Logistics (UPU S10).** A ubiquitous international tracking ID with
    a deterministic Mod 11 variant; complements your GS1 set and AWB.\
-   **Healthcare (NPI, DEA).** Common US identifiers with public
    checksum rules; high demand in healthcare/payments integrations.\
-   **Customs/Tax (EORI).** Needed for EU/UK trade workflows; baseline
    structural validation is useful even when country-specific checks
    aren't published.\
-   **Telecom (IMSI).** Pairs well with your existing ICCID/IMEI; even
    structure-only validation reduces bugs.\
-   **Identity/Travel (MRZ lines).** You already ship the MRZ checksum
    primitive---binding it to full TD1/TD3 documents rounds out the
    feature.

## API pattern (unchanged)

Continue your established pattern:

-   Static `Xxx.TryValidate(string? input, out XxxValue value)`\
-   Static
    `Xxx.TryGenerate(GenerationOptions options, Span<char> destination, out XxxValue value)`\
-   `XxxValue` as the normalized representation (and `ToString()`
    canonicalization).\
-   Country sub-namespaces where appropriate (e.g.,
    `Veritas.Finance.FR.Rib`).

## Acceptance criteria

-   ✅ Parsers & normalizers for each identifier.\
-   ✅ Deterministic check digit implementations with test vectors
    (positive/negative).\
-   ✅ Generators produce normalized, valid examples; seedable via
    `GenerationOptions`.\
-   ✅ XML docs with one-liner + example; added to DocFX TOC under
    existing sectors.\
-   ✅ Unit tests in
    `test/Veritas.Tests/<Sector>/<Country?>/<Identifier>Tests.cs`.\
-   ✅ Optional: register each identifier in a metadata catalog so
    docs/website can auto-list.

## Status

All identifiers listed above have been implemented.
