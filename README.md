# Veritas

[![Build](https://github.com/rebeccapowell/Veritas/actions/workflows/ci.yml/badge.svg)](https://github.com/rebeccapowell/Veritas/actions/workflows/ci.yml)
[![Docs](https://github.com/rebeccapowell/Veritas/actions/workflows/docs.yml/badge.svg)](https://github.com/rebeccapowell/Veritas/actions/workflows/docs.yml)
[![NuGet](https://img.shields.io/nuget/v/Veritas.svg)](https://www.nuget.org/packages/Veritas)

Core primitives and algorithms for identifier validation and generation.

> **⚠️ Caution**
> In active development. Please perform your own due diligence before deploying it in production. Feedback via GitHub issues and merge requests is always welcome.

## Install

```bash
dotnet add package Veritas --version 1.0.3
```

## Currently implemented

### Algorithms
- Luhn (mod 10 and base-36)
- ISO 7064 (mod 11,10; mod 97; mod 37,2; mod 37,36)
- GS1 mod 10
- Weighted mod 11 variants (custom weights)
- ISO 6346 container check digit
- MRZ (ICAO 9303 7-3-1 pattern)
- Base58Check codec
- Verhoeff checksum
- Damm checksum
- Checksum strategies via `IChecksum` interface and transliteration helpers

### Finance
- IBAN validation (ISO 13616 / ISO 7064 mod 97)
- BIC/SWIFT code structural validation
- ISIN validation (alphabetic + numeric with Luhn check digit)
- ISO 11649 RF creditor reference validation and generation
- Belgium structured communication (OGM) validation and generation
- SEPA Creditor Identifier (SCI) validation and generation
- French RIB key validation and generation
- Spanish CCC validation and generation
- Payment card PAN validation (Luhn)
- US ABA routing number validation
- Mexican CLABE validation and generation
- Legal Entity Identifier (LEI) validation and generation
- SEDOL validation and generation
- CUSIP validation and generation
- Market Identifier Code (MIC) structural validation
- German WKN structural validation

### Energy
- Energy Identification Code (EIC) validation and generation (ISO 7064 mod 37,2)
- Great Britain MPAN core validation and generation
- Great Britain MPRN validation
- Netherlands Energy EAN validation and generation
- Spain CUPS validation and generation
- Germany MaLo validation and generation
- Germany MeLo validation and generation
- Germany ZPN validation and generation
- France PRM validation
- Italy POD structural validation
- Italy PDR structural validation

### Identity
- UUID/GUID validation and generation
- Email address validation
- ULID validation and generation
- NanoID validation and generation
- Phone number (E.164) validation
- Domain name validation
- KSUID validation and generation
- BCP 47 language tag validation
- Ethereum address validation
- Base58Check validation and generation
- India Aadhaar validation and generation
- Luxembourg National ID validation and generation
- France NIR (INSEE) validation and generation
- ICAO 9303 MRZ passport lines validation and generation

### Tax
- Brazil CPF validation/generation
- Brazil CNPJ validation/generation
- Germany USt-IdNr validation/generation
- Germany IdNr validation/generation
- Canada SIN validation/generation
- Canada Business Number validation/generation
- United States SSN structural validation and generation
- United Kingdom NINO structural validation
- United Kingdom UTR checksum validation and generation
- United Kingdom VAT checksum validation
- United Kingdom Company Number structural validation
- United States EIN prefix validation
- United States ITIN structural validation
- Australia TFN checksum validation/generation
- Australia ABN checksum validation/generation
- India PAN validation/generation
- France SIREN validation
- France SIRET validation
- France VAT checksum validation
- Italy VAT (PIVA) checksum validation
- China USCC validation/generation
- Spain NIF checksum validation
- Spain NIE checksum validation
- Spain CIF checksum validation
- Netherlands BSN checksum validation
- Netherlands VAT (BTW) checksum validation
- Poland NIP checksum validation
- Poland REGON checksum validation
- Poland PESEL checksum validation
- Sweden Personnummer checksum validation
- Sweden OrgNr checksum validation
- New Zealand IRD checksum validation
- Argentina CUIT validation/generation
- Chile RUT validation/generation
- Belgium National Number validation/generation
- Portugal NIF validation/generation
- Greece AFM validation/generation
- Hungary VAT (Adószám) validation/generation
- Finland HETU validation/generation
- Ireland PPSN validation/generation
- Norway Fodselsnummer validation/generation
- Norway KID validation/generation
- Czech Rodne cislo validation/generation
- Slovakia Rodne cislo validation/generation
- Croatia OIB validation/generation
- Romania CNP validation/generation
- Bulgaria EGN validation/generation
- Slovenia EMSO validation/generation
- Serbia JMBG validation/generation
- Bosnia and Herzegovina JMBG validation/generation
- North Macedonia EMBG validation/generation
- Switzerland AHV validation/generation
- Switzerland UID validation/generation
- Austria UID validation/generation
- Iceland Kennitala validation/generation
- Lithuania Asmens kodas validation/generation
- Latvia Personas kods validation/generation
- Estonia Isikukood validation/generation
- Denmark CPR validation/generation
- Colombia NIT validation/generation
- Peru RUC validation/generation
- Turkey TCKN validation/generation
- Italy Codice Fiscale validation/generation
- Mexico RFC validation/generation
- Mexico CURP validation/generation
- Singapore UEN validation/generation
- South Africa National ID validation/generation
- Israel Teudat Zehut validation/generation
- EU/GB EORI validation and generation

### Logistics
- GTIN/EAN/UPC validation and generation (GS1 mod 10)
- Global Location Number (GLN) validation and generation
- Serial Shipping Container Code (SSCC) validation and generation
- Global Service Relation Number (GSRN) validation and generation
- Global Returnable Asset Identifier (GRAI) validation and generation
- Global Shipment Identification Number (GSIN) validation and generation
- Global Document Type Identifier (GDTI) validation and generation
- Global Individual Asset Identifier (GIAI) validation and generation
- UPU S10 postal tracking validation and generation
- Vehicle Identification Number (VIN) validation
- ISO 6346 container code validation
- Air Waybill (AWB) validation and generation
- IMO ship identification number validation and generation

### Telecom
- IMEI validation and generation
- IMSI validation and generation
- MEID validation and generation
- ICCID validation and generation
- MAC address validation and generation
- OUI validation and generation
- ASN validation and generation
- IPv4 structural validation
- IPv6 structural validation

### Intellectual Property
- Singapore IPOS application number validation and generation
- ISWC validation and generation
- Patent application number (WIPO ST.13) validation and generation
- Patent publication number (WIPO ST.16) validation and generation
- Trademark registration number validation and generation
- Copyright registration number validation and generation

### Legal
- European Case Law Identifier (ECLI) validation and generation
- U.S. court case number validation and generation
- European Patent Office publication identifier validation and generation

### Education & Media
- ISBN-10 validation and generation
- ISBN-13 validation and generation
- ISSN validation and generation
- DOI structural validation
- ISNI validation and generation
- ISMN validation and generation
- ISRC structural validation

### Healthcare
- NHS Number validation and generation
- ORCID validation and generation
- SNOMED CT Concept ID validation and generation
- NPI validation and generation
- DEA Number validation and generation

Additional identifiers and algorithms will be added per the [PRD](prds/PRD.md).

## Example usage
```csharp
// Validate an IBAN
var ok = Finance.Iban.TryValidate("FR14 2004 1010 0505 0001 3M02 606", out var iban);
Console.WriteLine(ok);                      // True
Console.WriteLine(iban.Value!.Value);       // FR1420041010050500013M02606

// Generate a GTIN-13
foreach (var s in Bulk.GenerateMany((dst, rng) => {
    var ok = Logistics.Gtin.TryGenerate(13, new GenerationOptions { Seed = rng.Next() }, dst, out var w);
    return (ok, w);
}, count: 3, seed: 42))
{
    Console.WriteLine(s);
}

// Validate a telecom identifier
var imeiOk = Telecom.Imei.TryValidate("490154203237518", out var imei);
Console.WriteLine(imeiOk);                  // True
```

## Contributing
See [contributors.md](contributors.md) for guidelines. In short, fork the repository, create a topic branch, and open a pull request:
1. Describe the motivation and design in the PR description.
2. Ensure tests and formatting checks pass.
3. The maintainers will review and merge when ready.

Continuous integration via [GitHub Actions](.github/workflows/ci.yml) restores dependencies, builds, and runs the test suite on every push and pull request. Packages are produced by the [publish workflow](.github/workflows/publish.yml) and releases are drafted automatically.

