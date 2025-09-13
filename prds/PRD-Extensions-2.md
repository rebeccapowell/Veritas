# PRD -- Country-Specific Identifiers (Deduplicated)

**Project:** Veritas\
**Version:** 3.0\
**Date:** 2025-09-13

## 1) Summary

Add high-value **national IDs/tax numbers** that are **not yet in
Veritas**, with formal checksum rules and safe test-data generators. No
duplicates with what's already listed in the README.

## 2) Goals

-   Broaden EU/EMEA/APAC/AMER coverage for IDs commonly used in tax,
    e-gov, payments, onboarding.
-   Reuse existing primitives (ISO 7064, mod-11 families, Luhn) already
    present in Veritas; add only what's missing.
-   Provide `TryValidate`, `TryGenerate`, and `Value` structs with
    normalization + formatting.

## 3) Algorithms

Most primitives already exist (ISO 7064 variants, GS1 mod-10, weighted
mod-11, Luhn) per README. No new core algorithms are strictly required
for this set. (We will add small **transliteration/weight maps** where a
scheme's tables differ by country, e.g., FI HETU Mod-31 map.)

## 4) Scope --- New Identifiers Only (not yet in README)

### 4.1 Europe (Priority: High)

  -------------------------------------------------------------------------------
  Country        Identifier      Format         Checksum       Notes
                                 (normalized)                  
  -------------- --------------- -------------- -------------- ------------------
  **BE**         National Number 11 digits      **mod-97**     Date-encoded; use
                 (NN)                                          synthetic dates
                                                               for generation.

  **PT**         NIF             9 digits       **mod-11**     Common for
                                                               invoicing.

  **GR**         AFM (Tax ID)    9 digits       **mod-11**     Powers-of-two
                                                (2\^weights)   weighting.

  **FI**         HETU            11 chars (6d + **mod-31** →   Support A/+/-
                                 sep + 3d +     check char     separators; letter
                                 check)                        map for check.

  **IE**         PPSN            7 digits +     **mod-23**     New 2-letter
                                 1--2 letters   weighted       suffix scheme.

  **NO**         Fødselsnummer   11 digits      **mod-11, two  Support D-numbers
                                                checks**       (offset day).

  **NO**         KID (OCR ref)   variable (up   **mod-10 or    Parameterized
                                 to 25)         mod-11**       variant.

  **CZ**         Rodné číslo     9/10 digits    **mod-11**     Normalize slash;
                                 (with slash)                  pre/post-1954
                                                               rules.

  **SK**         Rodné číslo     9/10 digits    **mod-11**     Similar to CZ.
                                 (with slash)                  

  **RO**         CNP             13 digits      **mod-11**     Country/sex/date
                                                               encoding;
                                                               synthetic dates
                                                               for generation.

  **BG**         EGN             10 digits      **weighted     Month offset
                                                checksum**     (20/40) handling.

  **HR**         OIB             11 digits      **ISO 7064 Mod Straightforward;
                                                11,10**        good quick win.

  **SI**         EMŠO            13 digits      **mod-11**     YYMMDD + region +
                                                               serial + check.

  **RS/BA/MK**   JMBG/EMBG       13 digits      **mod-11**     Regional variants;
                                                               same core logic.

  **CH**         AHV/AVS         13 digits      **mod-11**     Accept dotted
                                                               presentation;
                                                               output canonical.

  **CH**         UID (CHE)       CHE + 9 digits **mod-11**     Validate optional
                                 (+ MWST)                      VAT suffix.

  **AT**         UID (VAT)       ATU + 8 digits **weighted     Distinct weights
                                                mod-10**       from DE VAT.

  **IT**         Codice Fiscale  16 alnum       **position     Provide synthetic,
                                                table +        non-realistic
                                                control char** generator.

  **DK**         CPR             10 digits      *no official   Optional: format
                                                checksum*      validation only
                                                               (behind an
                                                               option).

  **IS**         Kennitala       10 digits      **mod-11**     Date-encoded;
                                                               synthetic dates.

  **LT**         Asmens kodas    11 digits      **mod-11**     Variant rules by
                                                (two-stage)    era; implement
                                                               both stages.

  **LV**         Personas kods   11 digits      **mod-11**     Date-encoded; dash
                                                               formatting
                                                               support.

  **EE**         Isikukood       11 digits      **mod-11**     Two-stage checksum
                                                               (k1/k2).
  -------------------------------------------------------------------------------

### 4.2 Americas

  ------------------------------------------------------------------------------
  Country        Identifier     Format         Checksum       Notes
  -------------- -------------- -------------- -------------- ------------------
  **AR**         CUIT/CUIL      11 digits      **mod-11**     Entity-type
                                                              prefixes.

  **CL**         RUT            digits + '-' + **mod-11**     "K" means 10.
                                check (0--9/K)                

  **CO**         NIT            9--10 + check  **mod-11**     Country-specific
                                                              weights.

  **PE**         RUC            11 digits      **weighted     Taxpayer registry.
                                               checksum**     

  **MX**         RFC            12/13 alnum    **check char** Homoclave
                                                              calculation.

  **MX**         CURP           18 alnum       **check char** Identity code.
  ------------------------------------------------------------------------------

### 4.3 APAC & MEA

  ----------------------------------------------------------------------------
  Country        Identifier     Format         Checksum       Notes
  -------------- -------------- -------------- -------------- ----------------
  **IN**         Aadhaar        12 digits      **Verhoeff**   Use synthetic
                                                              generation;
                                                              avoid realistic
                                                              links.

  **IN**         GSTIN          15 alnum       **base-36      Position-based
                                               check**        check character.

  **SG**         UEN            multiple       **weighted     Different
                                               checksum**     patterns by
                                                              entity type.

  **ZA**         National ID    13 digits      **Luhn**       Validate date +
                                                              citizenship +
                                                              check.

  **IL**         Teudat Zehut   9 digits       **weighted     Alt ×1/×2 and
                                               mod-10**       digit sums.

  **TR**         T.C. Kimlik    11 digits      **two parity   Sum rules on
                                               checks**       odd/even
                                                              positions.
  ----------------------------------------------------------------------------

## 5) API & Modeling (match existing Veritas style)

For each new identifier **X**: - `public static class X` -
`public static bool TryValidate(string input, out XValue value);` -
`public static bool TryGenerate(out XValue value, XGenerationOptions? options = null);` -
`public readonly struct XValue` - `.Raw` (normalized) - `.Formatted`
(local convention) - Optional `.CountryCode` - **Normalization:** strip
separators/whitespace, uppercase, enforce charset/length
post-normalize. - **Generation:** deterministic PRNG (seedable),
synthetic date ranges for DOB-encoded IDs, compute check digit(s).

## 6) Tests

-   Valid/invalid fixtures (official examples when available).
-   Edge cases (leading zeros, boundary dates, legacy patterns).
-   Round-trip property test: `TryGenerate → TryValidate`.
-   Formatting tests for `.Formatted`.

## 7) Docs

-   README capability matrix row per new ID (EN/DE blurb, example,
    checksum).
-   Per-type XML docs summarizing format + checksum.
-   "Test data only" disclaimer where appropriate.

## 8) Milestones

-   **M1 (EU Core):** BE NN, PT NIF, GR AFM, FI HETU, IE PPSN, NO
    Fødselsnummer/KID, CZ/SK Rodné, HR OIB.\
-   **M2 (EU Remainder):** RO CNP, BG EGN, SI EMŠO, RS/BA/MK JMBG/EMBG,
    CH AHV/UID, AT UID, IT CF, IS Kennitala, LT/LV/EE personal codes, DK
    CPR (format).\
-   **M3 (Americas):** AR CUIT, CL RUT, CO NIT, PE RUC, MX RFC/CURP.\
-   **M4 (APAC/MEA):** IN Aadhaar/GSTIN, SG UEN, ZA ID, IL TZ, TR TCKN.
