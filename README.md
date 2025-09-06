# Veritas

Core primitives and algorithms for identifier validation and generation.

## Currently implemented

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

### Tax
- Brazil CPF validation/generation
- Brazil CNPJ validation/generation
- Germany USt-IdNr validation/generation
- Germany IdNr validation/generation
- Canada SIN validation/generation
- Canada Business Number validation/generation
- United States SSN structural validation and generation
- United Kingdom NINO structural validation
- United Kingdom UTR checksum validation
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
- EU VAT structural validation
- Poland NIP checksum validation
- Poland REGON checksum validation
- Poland PESEL checksum validation
- Sweden Personnummer checksum validation
- Sweden OrgNr checksum validation
- New Zealand IRD checksum validation

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
- MEID validation and generation
- ICCID validation and generation
- MAC address validation and generation
- OUI validation and generation
- ASN validation and generation
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
- NHS Number validation and generation
- ORCID validation and generation

Additional identifiers and algorithms will be added per the [PRD](PRD.md).

## Example usage
```csharp
// Validate an IBAN
Finance.Iban.TryValidate("FR14 2004 1010 0505 0001 3M02 606", out var iban);
Console.WriteLine(iban.IsValid);            // True
Console.WriteLine(iban.Value!.Value);       // FR1420041010050500013M02606

// Generate a GTIN-13
foreach (var s in Bulk.GenerateMany(dst => {
    var ok = Logistics.Gtin13.TryGenerate("4006381".AsSpan(), dst, out var w);
    return (ok, w);
}, count: 3, seed: 42))
{
    Console.WriteLine(s);
}

// Validate a telecom identifier
Telecom.Imei.TryValidate("490154203237518", out var imei);
Console.WriteLine(imei.IsValid);            // True
```

## Project structure
```
src/Veritas/          core library organised by domain
  Core/              shared primitives and algorithms
  Finance/           financial identifiers
  Tax/               national and regional tax identifiers
  Energy/            electricity and gas identifiers
  Identity/          software and personal identifiers
  Logistics/         supply chain identifiers
  Healthcare/        health identifiers
  Telecom/           network and telecom identifiers
  Education/         research and publishing identifiers
  Media/             media identifiers

test/Veritas.Tests/   xUnit test suite
```

## Adding new identifiers
1. Add a new static class `<IdName>` in the appropriate domain with `TryValidate` and, when safe, `TryGenerate` methods.
2. Include a value type `<IdName>Value` containing the normalized representation.
3. Add unit tests under `test/Veritas.Tests` verifying valid and invalid cases.
4. Update the [PRD](PRD.md) and this README with the new identifier and its capabilities.
5. Run `dotnet test -f net8.0` and ensure all tests pass.

## Contributing
Contributions are welcome! Fork the repository, create a topic branch, and open a pull request:
1. Describe the motivation and design in the PR description.
2. Ensure tests and formatting checks pass.
3. The maintainers will review and merge when ready.

Continuous integration via [GitHub Actions](.github/workflows/ci.yml) restores dependencies, builds, and runs the test suite on every push and pull request. Packages are produced by the [publish workflow](.github/workflows/publish.yml) and releases are drafted automatically.
