# PRD-Extensions-4 --- Intellectual Property and Legal Identifiers

**Goal.** Extend Veritas with identifiers relating to **intellectual
property (copyright, patents, trademarks, publishing)** and **legal
field (case law, court, registries)**. This fills a current sector gap
and aligns with renaming the existing `IP` folder to
`IntellectualProperty` for clarity.

**Non-Goals.** No integration with external registries (e.g., WIPO,
EUIPO APIs). Validation is structural/checksum only, with safe test data
generation.

## At-a-glance table

  ----------------------------------------------------------------------------------------------
  sector                 country code   identifier       validation strategy --   generation
                                                         algorithm                strategy
  ---------------------- -------------- ---------------- ------------------------ --------------
  IntellectualProperty   --- (global)   **ISWC**         Structure: "T" + 9       Generate 9
                                        (International   digits + Mod 10 check    random digits,
                                        Standard Musical digit.                   compute Mod
                                        Work Code)                                10, prefix
                                                                                  with T.

  IntellectualProperty   --- (global)   **Patent         Structure: CC + YY +     Generate with
                                        Application      serial (max 6). No       ISO 3166 CC,
                                        Number (WIPO     checksum.                2-digit year,
                                        ST.13)**                                  random serial.

  IntellectualProperty   --- (global)   **Patent         Structure: CC + serial + Generate with
                                        Publication      kind code. No checksum.  valid CC and
                                        Number (WIPO                              kind code
                                        ST.16)**                                  (e.g., A1,
                                                                                  B1), random
                                                                                  serial.

  IntellectualProperty   --- (global)   **Trademark      Jurisdiction-specific    Generate
                                        Registration     formats (numeric or      structural
                                        Number**         alphanumeric). No        test IDs
                                                         checksum.                (prefix +
                                                                                  serial).

  IntellectualProperty   --- (global)   **Copyright      Jurisdiction-specific    Generate
                                        Registration     (e.g., US Copyright      structural
                                        Number**         Office). Structural      forms
                                                         rules only.              (prefix +
                                                                                  year +
                                                                                  serial).

  Legal                  EU             **ECLI**         Structure:               Generate with
                                        (European Case   ECLI:CC:COURT:YEAR:ID.   valid ISO
                                        Law Identifier)  No checksum.             country code,
                                                                                  sample court
                                                                                  ID, year,
                                                                                  numeric ID.

  Legal                  US             **Court Case     Format varies by         Generate
                                        Number**         district, e.g. YY-NNNNN  structural
                                                         TYPE. No checksum.       samples
                                                                                  (year + seq +
                                                                                  type).

  Legal                  EU             **European       EP + serial (6--7        Generate test
                                        Patent Office    digits) + kind code.     IDs with
                                        Publication ID** Structural only.         random serial
                                                                                  and valid kind
                                                                                  code.
  ----------------------------------------------------------------------------------------------

## Rationale & fit

-   **ISWC** --- checksum-based, widely used in copyright licensing
    (fits Veritas checksum model).\
-   **Patent identifiers (ST.13/ST.16)** --- structural validation,
    extremely common in IP systems.\
-   **Trademark / Copyright Registration Numbers** --- structural-only
    but high business relevance.\
-   **ECLI** --- widely adopted across EU courts, highly relevant for
    legal tech.\
-   **Court Case Numbers** --- less standardized, but even structural
    checks prevent common errors.\
-   **EPO Publication IDs** --- ties into patent workflows (structural
    only).

## API pattern

Follow established conventions:

-   `Xxx.TryValidate(string? input, out XxxValue value)`\
-   `Xxx.TryGenerate(GenerationOptions options, Span<char> destination, out XxxValue value)`\
-   `XxxValue` holds normalized representation.\
-   Namespaces: `Veritas.IntellectualProperty` and `Veritas.Legal`.

## Acceptance criteria

-   ✅ New folder: rename `IP/` → `IntellectualProperty/` in
    `/src/Veritas`.\
-   ✅ Add ISWC (with Mod 10 check digit).\
-   ✅ Add structural validators for ST.13, ST.16, Trademarks, Copyright
    registrations.\
-   ✅ Add ECLI (EU case law identifier).\
-   ✅ Add structural validator for Court Case Numbers (US baseline).\
-   ✅ Add European Patent Office Publication IDs.\
-   ✅ Unit tests for each in `test/Veritas.Tests/IntellectualProperty`
    and `Legal`.\
-   ✅ Update README + registry metadata.
