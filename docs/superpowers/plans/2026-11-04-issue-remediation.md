# Veritas issue remediation implementation plan

**Goal:** Address GitHub issues #22 to #29 in order, preserving the span-based public API conventions and independently reviewing each issue before proceeding.

**Architecture:** Correct existing validators at their parsing and checksum boundaries. Add new identifiers as separate value types and static validators/generators, sharing only independently tested algorithms. Synthetic generation never establishes official allocation.

**Spec:** GitHub issues https://github.com/rebeccapowell/Veritas/issues/22 through /29, read on 4 November 2026.

**Constraints:** Follow AGENTS.md, retain net8.0 consumer support, document public APIs, update README and DocFX, run tests and formatting before commits, open reviewable PRs without merging or publishing.

## Review focus

- Oversize and malformed Unicode input must return explicit failure without throwing or exhausting the stack.
- Checksum success must not bypass positional or country-specific structure checks.
- Every insufficient destination must return false and written = 0.
- Standards-derived fixtures must complement deterministic generator round trips.
- Offline validity and synthetic values must not imply assignment, ownership or production-safe identifiers.

## Execution order

1. **#22 input safety:** Reproduce short Base58 buffers, malformed Unicode and Plus Code bounds failures; inspect all input-sized stack buffers. Bound Base58 work and use bounded stack storage or safe variable storage in other affected parsers. Add adversarial and legitimate controls, run focused and full tests, request independent security review.
2. **#22 checksum correctness:** Correct EIC, MPAN, SEPA, FIGI, CUSIP and Italian tax-code calculations against primary specifications. Replace invalid positive fixtures. Add reference vectors and generator boundary tests before implementation.
3. **#22 other correctness:** Enforce IBAN/ISIN/MaLo structure, implement full/short/padded Plus Codes, correct KID and MRZ generation, update CNPJ/HETU, enforce ULID/KSUID bounds and generation contracts, implement RFC 5646 syntax. Test each affected parser/generator before proceeding.
4. **#22 maintenance:** Audit resolved dependencies, update compatible tooling, restore Dependabot, test supported runtimes and gate release publication on tests of the same commit. Pin actions/tooling where verifiable. Document remaining operational configuration separately. Review the complete issue diff before its PR.
5. **#23 MP-ID:** Confirm BDEW/DVGW positional and checksum rules; implement classified value, explicit scheme option, independent vectors, deterministic/short-buffer tests and documentation. Review and open a separate PR.
6. **#24 NeLo:** Verify E prefix and ASCII checksum; implement typed parser and synthetic generator with boundary tests and documentation. Review and open a separate PR.
7. **#25 NeBe:** Reuse the verified checksum with F prefix and wrong-type tests; document, review and open a separate PR.
8. **#26 resource family:** Implement A/B/C/D mapping with explicit expected/generated type, independent vectors, wrong-type rejection and documentation. Review and open a separate PR.
9. **#27 Leitweg-ID:** Verify KoSIT component grammar and MOD 97 conversion, implement component-aware generation with optional fine address, leading-zero and separator tests; document, review and open a separate PR.
10. **#28 W-IdNr.:** Verify base and suffix rules from authoritative sources; model both forms explicitly and avoid an unsubstantiated checksum rule. Add tests/docs, review and open a separate PR.
11. **#29 D-U-N-S:** Implement exactly nine ASCII digits and narrowly defined display form without invented checksum, preserve zeros, add deterministic and malformed-input tests/docs; review and open a separate PR.

For each deliverable: write the regression/specification tests first, observe the expected failures, implement, format changed C# files, run the full suite, obtain subagent review, resolve confirmed findings, rerun checks and commit. Record actual commands and limitations in PR descriptions. Stack dependent PRs where shared corrections are required, identifying their base explicitly.

## Execution ledger

- #22 safety: completed in local commit ffdfb9c. Initial six failing regression cases corrected; independent reviewer found BCP47 buffer-index regression, reproduced with three failing boundary tests and fixed. Full suite then 844 passed.
- #22 correctness: initial checksum corpus17 failures and modern-format corpus24 failures corrected; added SWIFT89 country patterns, calendar/omocodia structure and broad malformed-input checks across213 public validators. Reviewer found BG type-2 short payload; reproduced on both runtime targets and fixed. Explicit timestamp boundaries added.
- #22 maintenance: removed narrow AutoFixture string-fixture usage and vulnerable obsolete transitives, updated retained tooling, enabled net8/net10 tests, updated Actions release tags/DocFX, retained the disabled Dependabot configuration, isolated write jobs, gated release packing on tests. Build reviewer found mutable Docker-tag indirection in result publishing; switched to the Linux composite variant using a release tag.
- #22 verification: Release suite1145 passed on net8 and1145 on net10; feed reports zero vulnerable/outdated project packages after restore. NuGet pack succeeds with zero consumer dependencies. DocFX compiled-assembly metadata and HTML build succeed with zero warnings/errors. Normal project metadata and semantic formatter cannot start Roslyn's named-pipe host in this restricted environment; normal documentation build is configured for PR CI, and folder-based whitespace formatter succeeds locally.
- Compatibility rulings: affected parser raw resource limit4096; Base58 payload limit1024 bytes; EIC arbitrary generation disabled per ENTSO-E restriction; SEPA default synthetic generator emits DE18; seeded ULID/KSUID fixtures use epoch or explicit Timestamp. These are documented in README, DocFX and remediation notes.
- Operational follow-up: NuGet trusted-publishing trust policy/protected release environment require external owner configuration; existing API-key publication path retained. No publishing/merging authorised or performed.
- Next: #23 MP-ID, then #24-29. Keep separate issue PRs and subagent review gates, stack dependent PRs as necessary.

- #23 complete: MarketPartnerId classifies BDEW99 and DVGW98, validates allocation-mode bounds, and requires explicit generation scheme. Shared numeric checksum retains official MaLo vector. Reviewer found loss of checksum error in expected-scheme overload; failing test reproduced on both targets and fixed. Release1159/1159 on each target; DocFX metadata/HTML0 warnings/errors. Next#24.

- #24 complete: Nelo/NeloValue and shared alphanumeric helper implement the E-prefix scheme and letter-ASCII/numeric-digit checksum. Independent official A example and derived E vectors; 200 seeds, short buffers and hostile/Unicode tests. Long-s Unicode folding bypass reproduced and fixed by validating raw ASCII before folding. Subagent review: no findings. Release1172 on each target; DocFX0 warnings/errors. Next#25.

- #25 complete: Nebe/NebeValue enforce F-type and reuse reviewed ASCII helper. Independent F vectors, valid-E wrong-type regression,200seeds,Unicode/short/hostile tests. Subagent review:no findings. Release1186 each runtime and DocFX0warnings/errors. Next#26.

- #26 complete: ResourceId/ResourceIdType implement exact ACR/BSG/CSR/DTR mapping, parsed type, expected-type validation preserving checksum errors, explicit generation type, shared ASCII checksum. Independent mixed-body vectors and200seeds/type; invalid enums, wrong types, Unicode/hostile/short buffers. Subagent:no findings. Release1204 eachruntime; DocFX0warnings/errors. Next#27.

- #27 complete: Finance.DE.LeitwegId models coarse/optional fine/checkdigits and validates KoSIT2.0.2 component grammar and MOD97; provides explicit-component and optional-fine seeded generation, safe overlapping spans. Officialnumeric and independentalphanumeric/max fixtures,allvalidcoarselengths,short buffers and errorcategories. Reviewer:no blockingfindings; its recommended coverage added. Release1233eachruntime; DocFX0warnings/errors. Next#28, research base checksum evidence rather than assume VAT equivalence from shape.

- #28 structural portion complete: explicit WIdNr base/full structure methods and synthetic-structure generators, separate values, prefix/ASCII/suffix bounds/zero preservation. Independent evidence review found no primary universal algorithm specification; VAT reuse applies only to pre-existing VAT population and one ERiC fixture cannot identify an algorithm. No unqualified validator/generator or checksum claim is shipped. Full checksum work remains open in#28, with RefsonlyPR. Implementation reviewer:no findings. Release1246eachruntime; DocFX0warnings/errors. Next#29.
