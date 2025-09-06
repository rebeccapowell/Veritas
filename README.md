# Veritas

Core primitives and algorithms for identifier validation and generation.

Currently implemented:

### Algorithms
- Luhn (mod 10)
 - ISO 7064 (mod 11,10; mod 97; mod 37,2)
- GS1 mod 10
- Weighted mod 11 variants
- ISO 6346 container check digit
- MRZ (ICAO 9303 7-3-1 pattern)
- Base58Check codec

### Finance
- IBAN validation (ISO 13616 / ISO 7064 mod 97)
- BIC/SWIFT code structural validation
- ISIN validation (alphabetic + numeric with Luhn check digit)
- ISO 11649 RF creditor reference validation and generation
- Payment card PAN validation (Luhn)
- US ABA routing number validation
- Mexican CLABE validation and generation
- Legal Entity Identifier (LEI) validation and generation

### Energy
- Energy Identification Code (EIC) validation and generation (ISO 7064 mod 37,2)
- Great Britain MPAN core validation and generation
- Great Britain MPRN validation
- Netherlands Energy EAN validation and generation
- Spain CUPS validation and generation
- Germany MaLo validation and generation
- Germany MeLo validation and generation
- France PRM validation

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
- Base58Check encoding/decoding utilities

### Tax
- Brazil CPF validation/generation
- Brazil CNPJ validation/generation
- Germany USt-IdNr validation/generation
- Germany IdNr validation/generation
- United States SSN structural validation
- United Kingdom NINO structural validation
- United Kingdom UTR checksum validation
- United Kingdom VAT checksum validation
- United Kingdom Company Number structural validation
- United States EIN prefix validation
- United States ITIN structural validation
- France SIREN validation
- France SIRET validation
- France VAT checksum validation
- Italy VAT (PIVA) checksum validation
- Spain NIF checksum validation
- Spain NIE checksum validation
- Spain CIF checksum validation
- Netherlands BSN checksum validation
- Netherlands VAT (BTW) checksum validation
- EU VAT structural validation
- Poland NIP checksum validation
- Poland REGON checksum validation
- Poland PESEL checksum validation
- Sweden Personnummer checksum validation
- Sweden OrgNr checksum validation

### Logistics
- GTIN/EAN/UPC validation and generation (GS1 mod 10)
- Global Location Number (GLN) validation and generation
- Serial Shipping Container Code (SSCC) validation and generation
- Vehicle Identification Number (VIN) validation
- ISO 6346 container code validation
- Air Waybill (AWB) validation and generation
- IMO ship identification number validation and generation

### Telecom
- IMEI validation and generation
- ICCID validation and generation
- MAC address validation
- IPv4 structural validation
- IPv6 structural validation

### Education & Media
- ISBN-10 validation and generation
- ISBN-13 validation and generation
- ISSN validation and generation
- DOI structural validation
- ISNI validation and generation
- ISMN validation and generation
- ISRC structural validation

### Healthcare
- NHS Number validation
- ORCID validation

Additional identifiers and algorithms will be added per the [PRD](PRD.md).

## Development

Continuous integration is provided via [GitHub Actions](.github/workflows/ci.yml) which restores dependencies, builds, and runs the test suite on every push and pull request. Tagged releases trigger the [release workflow](.github/workflows/release.yml) to generate NuGet packages, publish them to nuget.org, and create GitHub releases with automatic release notes.

Community contributions are welcome—use the issue and pull request templates to report bugs or propose enhancements.
