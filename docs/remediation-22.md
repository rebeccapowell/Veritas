# Issue #22 remediation

This branch addresses the 16 findings in [issue #22](https://github.com/rebeccapowell/Veritas/issues/22). Offline validation establishes syntax and the implemented checksums, not assignment or ownership. Changes can reject previously accepted invalid values.

| Findings | Implementation and evidence |
| --- | --- |
| 1–3: resource safety and Try contracts | Fixed canonical buffers, 4,096-character parser resource caps, Base58 1,024-byte payload cap before conversion, safe short destinations, ASCII checksum input, and bounds checks. Regression tests include five-million-character hostile input, maximum Base58 payloads, Unicode, and every insufficient generator buffer. |
| 4–9: checksums | Standards-derived EIC, MPAN, SEPA, FIGI, CUSIP and Italian tax-code fixtures accompany corrections to the independent algorithms. EIC unrestricted generation is unavailable because the source restricts allocation-related use to authorised issuing offices. |
| 10: structure | All 89 BBAN patterns in the downloaded SWIFT registry are checked before IBAN arithmetic; ISIN prefix/check character and MaLo first-digit rules are enforced. |
| 11–13: generators | Full/short/padded Plus Codes, valid geographic prefix generation, KID forbidden-remainder failure, and MRZ YY generation. |
| 14: current national formats | Alphanumeric CNPJ uses the published ASCII-minus-48 conversion; HETU accepts additional century markers and validates calendar dates. |
| 15: identity ranges and randomness | ULID and KSUID reject overflow; unseeded ULID/KSUID/NanoID use cryptographic randomness. Seeded timestamp-bearing fixtures use a fixed epoch or explicit timestamp. |
| 16: language tags | RFC 5646 well-formed syntax, duplicate singleton/variant rejection and grandfathered/private-use support, independent of installed cultures. |

## Resolved dependency audit

Audited on 4 November 2026 using `dotnet list Veritas.sln package --vulnerable --include-transitive --no-restore --format json` after a successful restore. Before remediation the feed reported:

| Scope | Package | Version | Advisory |
| --- | --- | --- | --- |
| Private build tooling | Microsoft.Build.Tasks.Git | 8.0.0 | GHSA-23fw-v26w-5fgq, moderate |
| Tests | System.Net.Http | 4.3.0 | GHSA-7jgj-8wvc-jh57, high |
| Tests | System.Text.RegularExpressions | 4.3.0 | GHSA-cmhx-cq75-c4mj, high |

SourceLink is updated to 10.0.403. AutoFixture was used only to feed arbitrary strings to nine negative tests; explicit malformed cases now replace that use, removing its obsolete transitive dependencies. The unused direct xunit.abstractions reference is removed. Test SDK 18.11.0, xunit 2.9.3, adapter 4.0.1, Shouldly 4.3.0 and coverlet.collector 10.1.0 are resolved and tested. The post-change audit reports no vulnerable packages, and the outdated check reports no newer stable versions for the retained project references. This is a feed-based result at the stated time, not proof that vulnerabilities cannot exist. No third-party runtime dependencies are shipped to library consumers.

NuGet audit includes transitive packages and fails restore for reported vulnerability severities. Dependabot covers NuGet and Actions. Tests target net8.0 and net10.0; the library retains its net8.0 target. Publishing tests the checked-out release commit before packaging, and deployment depends on that successful job. Write permissions are isolated to result publishing and documentation deployment. Actions are pinned to upstream commit SHAs and DocFX to 2.81.0.

## Validation

Use `dotnet test Veritas.sln --configuration Release` for both runtime targets, `dotnet format` for style, `dotnet tool restore`, and `dotnet tool run docfx docfx/docfx.json` for documentation. In the restricted development environment MSBuild is run with `--disable-build-servers -m:1 -p:UseSharedCompilation=false`. Named-pipe restrictions prevent Roslyn's project-loading host, so local formatting uses `dotnet format whitespace . --folder` and documentation is additionally checked from compiled assembly metadata. The PR documentation workflow runs the normal project-based build.

## Operational follow-up

Trusted publishing requires a trust policy configured on NuGet.org and a protected GitHub release environment. This branch retains the existing API-key publication path so it does not break releases before that external configuration exists. No package is published by this work. Repository branch protections and release-environment reviewers should be configured by the repository owner as appropriate.

## Specification references

- [ENTSO-E EIC implementation guide 1.1, section 5](https://eepublicdownloads.entsoe.eu/clean-documents/EDI/Library/cim_based/EIC_Data_Exchange_IG_v1.1.pdf)
- [Ofgem FIT guidance, MPAN check-digit calculation](https://www.ofgem.gov.uk/sites/default/files/2025-09/FIT-Guidance-for-Licensed-Electricity-Suppliers-V17.3.pdf)
- [Bundesbank creditor identifiers](https://www.bundesbank.de/en/tasks/payment-systems/services/sepa/creditor-identifier) and [EPC national structure overview 12.0](https://www.europeanpaymentscouncil.eu/sites/default/files/kb/file/2025-10/EPC262-08%20v12.0%20Creditor%20Identifier%20Overview.pdf)
- [OpenFIGI calculation](https://www.openfigi.com/insights/reference-materials/2018/6/15/figi-check-digit-calculation) and [allocation examples](https://www.openfigi.com/about/allocation-rules)
- [SWIFT IBAN registry](https://www.swift.com/swift-resource/11971/download)
- [Italian tax-code mapping](https://def.finanze.it/DocTribFrontend/executePrintArticolo.do?articolo=Allegato+A&codiceOrdinamento=60A+++++0000000&id=%7B04AB9404-46E7-4072-B414-892639727A96%7D)
- [BDEW identifier rules 1.2](https://www.bdew.de/media/documents/AWH_Identifikatoren-in-der-Marktkommunikation_Version.1.2.pdf)
- [Google Open Location Code reference](https://github.com/google/open-location-code/blob/main/python/openlocationcode/openlocationcode.py)
- [Receita Federal alphanumeric CNPJ conversion](https://normas.receita.fazenda.gov.br/sijut2consulta/anexoOutros.action?idArquivoBinario=76204)
- [Finnish tax administration identity-code rules](https://www.vero.fi/globalassets/tietoa-verohallinnosta/ohjelmistokehittajille/finnish-tax-administration_the-control-character-for-verifying-the-authenticity-of-finnish-business-ids-and-personal-identity-codes.pdf)
- [ULID specification](https://github.com/ulid/spec), [KSUID reference](https://github.com/segmentio/ksuid), [BCP 47 grammar](https://www.rfc-editor.org/rfc/rfc5646)

## Final integration follow-up

The final acceptance check added MPAN distributor syntax and constrained synthetic generation to known distributor 12. It preserves codes 29/30 supported by EMRS WP25 instead of freezing the outdated Ofgem upper range 28; actual live allocation is not checked offline. The final integration reviewer also identified CNPJ's Unicode long-s casefold acceptance. A compiled regression reproduced it on both runtimes; raw ASCII is now checked before case conversion while ASCII lower-case input remains supported.
