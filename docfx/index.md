---
_layout: landing
---

<img src="images/veritas.png" alt="Veritas logo" width="200" />

# Veritas

[![Build](https://github.com/rebeccapowell/Veritas/actions/workflows/ci.yml/badge.svg)](https://github.com/rebeccapowell/Veritas/actions/workflows/ci.yml)
[![Docs](https://github.com/rebeccapowell/Veritas/actions/workflows/docs.yml/badge.svg)](https://github.com/rebeccapowell/Veritas/actions/workflows/docs.yml)
[![NuGet](https://img.shields.io/nuget/v/Veritas.svg)](https://www.nuget.org/packages/Veritas)

Veritas is a .NET library for validating and generating a wide range of real‑world identifiers across finance, tax, energy, identity, logistics and more.

> **⚠️ Caution**
> Most of this code was agentically coded. Review and test thoroughly before relying on it in production. Feedback via GitHub issues and merge requests is greatly appreciated.

## Install

```bash
dotnet add package Veritas --version 1.0.3
```

## Usage

```csharp
// Validate an IBAN
var ok = Finance.Iban.TryValidate("FR14 2004 1010 0505 0001 3M02 606", out var iban);
Console.WriteLine(ok);                      // True

// Generate an IMEI
Telecom.Imei.TryGenerate(default, stackalloc char[15], out var written);

// Bulk generate GTIN-13 codes
foreach (var s in Bulk.GenerateMany((dst, rng) => {
    var ok = Logistics.Gtin.TryGenerate(13, new GenerationOptions { Seed = rng.Next() }, dst, out var w);
    return (ok, w);
}, count: 3, seed: 42))
{
    Console.WriteLine(s);
}
```

## Algorithms

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

## Supported identifiers

### Finance

| Identifier | Country/Region | Validation & Generation | Docs |
|------------|----------------|-------------------------|------|
| IBAN | International | ISO 7064 mod 97; BBAN-based generation | <xref:Veritas.Finance.Iban> |
| BIC/SWIFT | International | format check | <xref:Veritas.Finance.Bic> |
| ISIN | International | Luhn checksum | <xref:Veritas.Finance.Isin> |
| RF Creditor Reference | International | ISO 11649 mod 97; generation | <xref:Veritas.Finance.Rf> |
| SEPA Creditor Identifier | EU | ISO 7064 mod 97 checksum; generation | <xref:Veritas.Finance.SepaCreditorIdentifier> |
| Structured Reference (OGM) | BE | mod 97 checksum; generation | <xref:Veritas.Finance.BE.Ogm> |
| RIB | FR | mod 97 checksum; generation | <xref:Veritas.Finance.FR.Rib> |
| CCC | ES | dual mod 11 checksum; generation | <xref:Veritas.Finance.ES.Ccc> |
| Payment card PAN | International | Luhn checksum; test generation | <xref:Veritas.Finance.Pan> |
| ABA Routing | US | weighted mod 11 checksum | <xref:Veritas.Finance.AbaRouting> |
| CLABE | Mexico | weighted mod 11 checksum; generation | <xref:Veritas.Finance.Clabe> |
| LEI | International | ISO 7064 mod 97 checksum; generation | <xref:Veritas.Finance.Lei> |
| SEDOL | UK | weighted mod 10 checksum; generation | <xref:Veritas.Finance.Sedol> |
| CUSIP | US/Canada | weighted mod 10 checksum; generation | <xref:Veritas.Finance.Cusip> |
| Market Identifier Code (MIC) | Global | structural check | <xref:Veritas.Finance.MarketIdentifierCode> |
| WKN | Germany | structural check | <xref:Veritas.Finance.Wkn> |
| FIGI | Global | character-weighted check digit; synthetic generation | <xref:Veritas.Finance.Figi> |
| CHIPS Participant ID | US | format check; generation | <xref:Veritas.Finance.ChipsParticipantId> |

### Energy

| Identifier | Country/Region | Validation & Generation | Docs |
|------------|----------------|-------------------------|------|
| EIC | International | ENTSO-E weighted checksum; validation only | <xref:Veritas.Energy.Eic> |
| MPAN | GB | weighted mod 11 checksum; generation | <xref:Veritas.Energy.GB.Mpan> |
| MPRN | GB | length & format | <xref:Veritas.Energy.GB.Mprn> |
| Energy EAN | NL | GS1 mod 10 checksum; generation | <xref:Veritas.Energy.NL.EnergyEan> |
| CUPS | ES | mod 529 checksum; generation | <xref:Veritas.Energy.ES.Cups> |
| MaLo | DE | weighted checksum; generation | <xref:Veritas.Energy.DE.Malo> |
| MeLo | DE | weighted checksum; generation | <xref:Veritas.Energy.DE.Melo> |
| ZPN | DE | weighted checksum; generation | <xref:Veritas.Energy.DE.Zpn> |
| PRM | FR | structural check | <xref:Veritas.Energy.FR.Prm> |
| POD | IT | structural check | <xref:Veritas.Energy.IT.Pod> |
| PDR | IT | structural check | <xref:Veritas.Energy.IT.Pdr> |

### Identity

| Identifier | Country/Region | Validation & Generation | Docs |
|------------|----------------|-------------------------|------|
| UUID/GUID | Global | structural check; generation | <xref:Veritas.Identity.Uuid> |
| ULID | Global | Crockford base32 checksum; generation | <xref:Veritas.Identity.Ulid> |
| NanoID | Global | custom alphabet; generation | <xref:Veritas.Identity.NanoId> |
| KSUID | Global | base62; generation | <xref:Veritas.Identity.Ksuid> |
| Email | Global | syntax check | <xref:Veritas.Identity.Email> |
| Phone | Global | E.164 length/format | <xref:Veritas.Identity.Phone> |
| Domain | Global | hostname rules | <xref:Veritas.Identity.Domain> |
| BCP47 tag | Global | structural check | <xref:Veritas.Identity.Bcp47> |
| Base58Check | Global | Base58 + double SHA-256 | <xref:Veritas.Identity.Base58Check> |
| Aadhaar | India | Verhoeff checksum; test generation | <xref:Veritas.Identity.India.Aadhaar> |
| National ID | Luxembourg | Luhn + Verhoeff; test generation | <xref:Veritas.Identity.Luxembourg.NationalId> |
| CURP | Mexico | mod 10 checksum; generation | <xref:Veritas.Identity.Mexico.Curp> |
| National ID | South Africa | Luhn checksum; generation | <xref:Veritas.Identity.SouthAfrica.NationalId> |
| Teudat Zehut | Israel | weighted mod 10 checksum; generation | <xref:Veritas.Identity.Israel.TeudatZehut> |
| NIR (INSEE) | France | mod 97 key; generation | <xref:Veritas.Identity.France.Nir> |
| ICAO MRZ (TD3) | Global | MRZ checksums; generation | <xref:Veritas.Identity.IcaoMrz> |

### Tax

| Country | Identifier | Validation & Generation | Docs |
|---------|------------|-------------------------|------|
| EU/GB | EORI | structural check; generation | <xref:Veritas.Tax.Eori> |
| AR | CUIT | mod 11 checksum; generation | <xref:Veritas.Tax.AR.Cuit> |
| AU | ABN | weighted mod 11 checksum; generation | <xref:Veritas.Tax.AU.Abn> |
| AU | TFN | weighted mod 11 checksum; generation | <xref:Veritas.Tax.AU.Tfn> |
| BE | National Number | mod 97 checksum; generation | <xref:Veritas.Tax.BE.Nn> |
| BR | CPF | mod 11 checksum; generation | <xref:Veritas.Tax.BR.Cpf> |
| BR | CNPJ | mod 11 checksum; generation | <xref:Veritas.Tax.BR.Cnpj> |
| CA | SIN | Luhn checksum; generation | <xref:Veritas.Tax.CA.Sin> |
| CA | BN | mod 11 checksum; generation | <xref:Veritas.Tax.CA.Bn> |
| CL | RUT | mod 11 checksum; generation | <xref:Veritas.Tax.CL.Rut> |
| CN | USCC | weighted mod 31 checksum; generation | <xref:Veritas.Tax.CN.Uscc> |
| CZ | Rodne cislo | mod 11 checksum; generation | <xref:Veritas.Tax.CZ.RodneCislo> |
| DE | UStIdNr | mod 97 checksum; generation | <xref:Veritas.Tax.DE.UstIdNr> |
| DE | IdNr | mod 11 checksum; generation | <xref:Veritas.Tax.DE.IdNr> |
| ES | NIF | mod 11 checksum; generation | <xref:Veritas.Tax.ES.Nif> |
| ES | NIE | mod 11 checksum; generation | <xref:Veritas.Tax.ES.Nie> |
| ES | CIF | mod 11 checksum; generation | <xref:Veritas.Tax.ES.Cif> |
| FI | HETU | mod 31 checksum; generation | <xref:Veritas.Tax.FI.Hetu> |
| FR | Siren | mod 11 checksum | <xref:Veritas.Tax.FR.Siren> |
| FR | Siret | mod 11 checksum | <xref:Veritas.Tax.FR.Siret> |
| FR | VAT | ISO 7064 mod 97 checksum | <xref:Veritas.Tax.FR.Vat> |
| GR | AFM | mod 11 checksum; generation | <xref:Veritas.Tax.GR.Afm> |
| HU | Adoszam | weighted mod 10 checksum; generation | <xref:Veritas.Tax.HU.Adoszam> |
| HR | OIB | ISO 7064 mod 11,10 checksum; generation | <xref:Veritas.Tax.HR.Oib> |
| RO | CNP | mod 11 checksum; generation | <xref:Veritas.Tax.RO.Cnp> |
| BG | EGN | weighted checksum; generation | <xref:Veritas.Tax.BG.Egn> |
| SI | EMSO | mod 11 checksum; generation | <xref:Veritas.Tax.SI.Emso> |
| RS | JMBG | mod 11 checksum; generation | <xref:Veritas.Tax.RS.Jmbg> |
| BA | JMBG | mod 11 checksum; generation | <xref:Veritas.Tax.BA.Jmbg> |
| MK | EMBG | mod 11 checksum; generation | <xref:Veritas.Tax.MK.Embg> |
| CH | AHV | ISO 7064 mod 11,10 checksum; generation | <xref:Veritas.Tax.CH.Ahv> |
| CH | UID | mod 10 (EAN) checksum; generation | <xref:Veritas.Tax.CH.Uid> |
| AT | UID | weighted mod 10 checksum; generation | <xref:Veritas.Tax.AT.Uid> |
| IS | Kennitala | mod 11 checksum; generation | <xref:Veritas.Tax.IS.Kennitala> |
| LT | Asmens kodas | two-stage mod 11 checksum; generation | <xref:Veritas.Tax.LT.AsmensKodas> |
| LV | Personas kods | mod 11 checksum; generation | <xref:Veritas.Tax.LV.PersonasKods> |
| EE | Isikukood | two-stage mod 11 checksum; generation | <xref:Veritas.Tax.EE.Isikukood> |
| DK | CPR | structural check; generation | <xref:Veritas.Tax.DK.Cpr> |
| CO | NIT | mod 11 checksum; generation | <xref:Veritas.Tax.CO.Nit> |
| PE | RUC | weighted checksum; generation | <xref:Veritas.Tax.PE.Ruc> |
| TR | TCKN | parity checks; generation | <xref:Veritas.Tax.TR.Tckn> |
| IT | Codice Fiscale | position table check; generation | <xref:Veritas.Tax.IT.CodiceFiscale> |
| MX | RFC | mod 11 check; generation | <xref:Veritas.Tax.MX.Rfc> |
| SG | UEN | mod 11 check; generation | <xref:Veritas.Tax.SG.Uen> |
| IE | PPSN | mod 23 checksum; generation | <xref:Veritas.Tax.IE.Ppsn> |
| IN | PAN | mod 36 checksum; generation | <xref:Veritas.Tax.IN.Pan> |
| IN | GSTIN | mod 36 checksum; generation | <xref:Veritas.Tax.IN.Gstin> |
| IT | PIVA | mod 11 checksum | <xref:Veritas.Tax.IT.Piva> |
| NL | BSN | mod 11 checksum | <xref:Veritas.Tax.NL.Bsn> |
| NL | BTW | mod 11 checksum | <xref:Veritas.Tax.NL.Btw> |
| NO | Fodselsnummer | mod 11 checksum; generation | <xref:Veritas.Tax.NO.Fodselsnummer> |
| NO | KID | mod 10/11 checksum; generation | <xref:Veritas.Tax.NO.Kid> |
| NZ | IRD | mod 11 checksum | <xref:Veritas.Tax.NZ.Ird> |
| PL | NIP | mod 11 checksum | <xref:Veritas.Tax.PL.Nip> |
| PL | REGON | mod 11 checksum | <xref:Veritas.Tax.PL.Regon> |
| PL | PESEL | weighted mod 10 checksum | <xref:Veritas.Tax.PL.Pesel> |
| PT | NIF | mod 11 checksum; generation | <xref:Veritas.Tax.PT.Nif> |
| SE | Personnummer | Luhn checksum | <xref:Veritas.Tax.SE.Personnummer> |
| SE | OrgNr | mod 10 checksum | <xref:Veritas.Tax.SE.OrgNr> |
| SK | Rodne cislo | mod 11 checksum; generation | <xref:Veritas.Tax.SK.RodneCislo> |
| UK | NINO | structural check | <xref:Veritas.Tax.UK.Nino> |
| UK | UTR | mod 97 checksum; generation | <xref:Veritas.Tax.UK.Utr> |
| UK | VAT | mod 97 checksum | <xref:Veritas.Tax.UK.Vat> |
| UK | Company Number | structural check | <xref:Veritas.Tax.UK.CompanyNumber> |
| US | SSN | structural check; generation | <xref:Veritas.Tax.US.Ssn> |
| US | EIN | prefix check | <xref:Veritas.Tax.US.Ein> |
| US | ITIN | structural check | <xref:Veritas.Tax.US.Itin> |

### Logistics

| Identifier | Country/Region | Validation & Generation | Docs |
|------------|----------------|-------------------------|------|
| GTIN/EAN/UPC | International | GS1 mod 10 checksum; generation | <xref:Veritas.Logistics.Gtin> |
| GLN | International | GS1 mod 10 checksum; generation | <xref:Veritas.Logistics.Gln> |
| SSCC | International | GS1 mod 10 checksum; generation | <xref:Veritas.Logistics.Sscc> |
| GSRN | International | GS1 mod 10 checksum; generation | <xref:Veritas.Logistics.Gsrn> |
| GRAI | International | GS1 mod 10 checksum; generation | <xref:Veritas.Logistics.Grai> |
| GSIN | International | GS1 mod 10 checksum; generation | <xref:Veritas.Logistics.Gsin> |
| GDTI | International | GS1 mod 10 checksum; generation | <xref:Veritas.Logistics.Gdti> |
| GIAI | International | structural check; generation | <xref:Veritas.Logistics.Giai> |
| UPU S10 | International | weighted mod 11 checksum; generation | <xref:Veritas.Logistics.UpuS10> |
| VIN | International | transliteration + weighted checksum | <xref:Veritas.Logistics.Vin> |
| ISO 6346 | International | ISO 6346 checksum | <xref:Veritas.Logistics.Iso6346> |
| AWB | International | mod 7 checksum; generation | <xref:Veritas.Logistics.Awb> |
| IMO | International | mod 10 checksum; generation | <xref:Veritas.Logistics.Imo> |

### Telecom

| Identifier | Country/Region | Validation & Generation | Docs |
|------------|----------------|-------------------------|------|
| IMEI | Global | Luhn checksum; generation | <xref:Veritas.Telecom.Imei> |
| IMSI | Global | structural check; generation | <xref:Veritas.Telecom.Imsi> |
| MEID | Global | Luhn checksum; generation | <xref:Veritas.Telecom.Meid> |
| ICCID | Global | Luhn checksum; generation | <xref:Veritas.Telecom.Iccid> |
| MAC | Global | format + OUI | <xref:Veritas.Telecom.Mac> |
| OUI | Global | format check | <xref:Veritas.Telecom.Oui> |
| ASN | Global | range check; generation | <xref:Veritas.Telecom.Asn> |
| IPv4 | Global | structural check | <xref:Veritas.Telecom.Ipv4> |
| IPv6 | Global | structural check | <xref:Veritas.Telecom.Ipv6> |

### Intellectual Property

| Identifier | Country/Region | Validation & Generation | Docs |
|------------|----------------|-------------------------|------|
| IPOS Application Number | Singapore | Damm checksum; test generation | <xref:Veritas.IntellectualProperty.SG.IpApplicationNumber> |
| ISWC | International | mod 10 checksum; generation | <xref:Veritas.IntellectualProperty.Iswc> |
| Patent Application Number | International | structural check; generation | <xref:Veritas.IntellectualProperty.PatentApplicationNumber> |
| Patent Publication Number | International | structural check; generation | <xref:Veritas.IntellectualProperty.PatentPublicationNumber> |
| Trademark Registration Number | International | structural check; generation | <xref:Veritas.IntellectualProperty.TrademarkRegistrationNumber> |
| Copyright Registration Number | International | structural check; generation | <xref:Veritas.IntellectualProperty.CopyrightRegistrationNumber> |

### Legal

| Identifier | Country/Region | Validation & Generation | Docs |
|------------|----------------|-------------------------|------|
| European Case Law Identifier (ECLI) | EU | structural check; generation | <xref:Veritas.Legal.EU.Ecli> |
| Court Case Number | US | structural check; generation | <xref:Veritas.Legal.US.CourtCaseNumber> |
| European Patent Office Publication ID | EU | structural check; generation | <xref:Veritas.Legal.EU.EuropeanPatentOfficePublicationId> |

### Education & Media

| Identifier | Country/Region | Validation & Generation | Docs |
|------------|----------------|-------------------------|------|
| ISBN-10 | International | weighted checksum; generation | <xref:Veritas.Media.Isbn10> |
| ISBN-13 | International | GS1 mod 10 checksum; generation | <xref:Veritas.Media.Isbn13> |
| ISSN | International | weighted checksum; generation | <xref:Veritas.Media.Issn> |
| DOI | International | structural check | <xref:Veritas.Education.Doi> |
| ISNI | International | ISO 7064 mod 11,2 checksum; generation | <xref:Veritas.Education.Isni> |
| ISMN | International | mod 10 checksum; generation | <xref:Veritas.Education.Ismn> |
| ISRC | International | structural check | <xref:Veritas.Media.Isrc> |
| ROR ID | International | structural check; generation | <xref:Veritas.Education.RorId> |
| Scopus Author ID | International | structural check; generation | <xref:Veritas.Education.ScopusAuthorId> |
| ResearcherID | International | structural check; generation | <xref:Veritas.Education.ResearcherId> |
| GRID ID | International | structural check; generation | <xref:Veritas.Education.GridId> |

### Healthcare

| Identifier | Country/Region | Validation & Generation | Docs |
|------------|----------------|-------------------------|------|
| NHS Number | UK | mod 11 checksum; generation | <xref:Veritas.Healthcare.NhsNumber> |
| ORCID | International | ISO 7064 mod 11,2 checksum; generation | <xref:Veritas.Healthcare.Orcid> |
| SNOMED CT Concept ID | International | Verhoeff checksum; test generation | <xref:Veritas.Healthcare.Snomed.SctId> |
| NPI | US | Luhn checksum; generation | <xref:Veritas.Healthcare.Npi> |
| DEA Number | US | weighted checksum; generation | <xref:Veritas.Healthcare.DeaNumber> |
| Unique Device Identifier (UDI) | Global | format check; generation | <xref:Veritas.Healthcare.Udi> |
| National Drug Code (NDC) | US | structural check; generation | <xref:Veritas.Healthcare.Ndc> |
| ICD Code | International | format check; generation | <xref:Veritas.Healthcare.IcdCode> |
| RxNorm Identifier | US | structural check; generation | <xref:Veritas.Healthcare.RxNormIdentifier> |
| NDC Package Code | US | structural check; generation | <xref:Veritas.Healthcare.NdcPackageCode> |

### Transportation / Vehicles

| Identifier | Country/Region | Validation & Generation | Docs |
|------------|----------------|-------------------------|------|
| FAA N-number | US | format check; generation | <xref:Veritas.Transportation.FaaNNumber> |
| ICAO Airline Code | International | format check; generation | <xref:Veritas.Transportation.IcaoAirlineCode> |
| IATA Airline Code | International | format check; generation | <xref:Veritas.Transportation.IataAirlineCode> |
| Flight Number | International | format check; generation | <xref:Veritas.Transportation.FlightNumber> |
| Train UIC Number | International | mod 11 checksum; generation | <xref:Veritas.Transportation.TrainUicNumber> |
| IMO Call Sign | International | format check; generation | <xref:Veritas.Transportation.ImoCallSign> |

### Crypto / Blockchain

| Identifier | Country/Region | Validation & Generation | Docs |
|------------|----------------|-------------------------|------|
| Bitcoin address | Global | Base58 + double SHA-256 | <xref:Veritas.Crypto.BitcoinAddress> |
| Ethereum transaction hash | Global | structural check; generation | <xref:Veritas.Crypto.EthereumTransactionHash> |
| Chain ID | Global | numeric; generation | <xref:Veritas.Crypto.ChainId> |

### Government / Public Administration

| Identifier | Country/Region | Validation & Generation | Docs |
|------------|----------------|-------------------------|------|
| Passport Number | Global | format check; generation | <xref:Veritas.Government.PassportNumber> |
| Visa Number | Global | format check; generation | <xref:Veritas.Government.VisaNumber> |
| Driver Licence Number (CA) | US | structural check; generation | <xref:Veritas.Government.US.CA.DriverLicenseNumber> |
| Driver Licence Number (NY) | US | structural check; generation | <xref:Veritas.Government.US.NY.DriverLicenseNumber> |
| Driver Licence Number (FL) | US | structural check; generation | <xref:Veritas.Government.US.FL.DriverLicenseNumber> |
| Driver Licence Number (TX) | US | structural check; generation | <xref:Veritas.Government.US.TX.DriverLicenseNumber> |
| Driver Licence Number (ON) | CA | structural check; generation | <xref:Veritas.Government.CA.ON.DriverLicenceNumber> |
| Driver Licence Number (BC) | CA | structural check; generation | <xref:Veritas.Government.CA.BC.DriverLicenceNumber> |
| Driver Licence Number (QC) | CA | structural check; generation | <xref:Veritas.Government.CA.QC.DriverLicenceNumber> |
| Driver Licence Number (NSW) | AU | structural check; generation | <xref:Veritas.Government.AU.NSW.DriverLicenceNumber> |
| Driver Licence Number (VIC) | AU | structural check; generation | <xref:Veritas.Government.AU.VIC.DriverLicenceNumber> |
| Driver Licence Number (QLD) | AU | structural check; generation | <xref:Veritas.Government.AU.QLD.DriverLicenceNumber> |
| Driver Licence Number (WA) | AU | structural check; generation | <xref:Veritas.Government.AU.WA.DriverLicenceNumber> |
| Licencia de Conducir (CDMX) | MX | structural check; generation | <xref:Veritas.Government.MX.CDMX.LicenciaDeConducir> |
| Licencia de Conducir (JAL) | MX | structural check; generation | <xref:Veritas.Government.MX.JAL.LicenciaDeConducir> |
| Licencia de Conducir (NLE) | MX | structural check; generation | <xref:Veritas.Government.MX.NLE.LicenciaDeConducir> |
| Licencia de Conducir (BCN) | MX | structural check; generation | <xref:Veritas.Government.MX.BCN.LicenciaDeConducir> |
| Führerscheinnummer | DE | structural check; generation | <xref:Veritas.Government.DE.Fuehrerscheinnummer> |
| Permis de Conduire | FR | structural check; generation | <xref:Veritas.Government.FR.PermisDeConduire> |
| Patente di guida | IT | structural check; generation | <xref:Veritas.Government.IT.PatenteDiGuida> |
| Rijbewijsnummer | NL | structural check; generation | <xref:Veritas.Government.NL.Rijbewijsnummer> |
| Körkortsnr | SE | format check; generation | <xref:Veritas.Government.SE.Korkortsnr> |
| Driving Licence Number | IN | structural check; generation | <xref:Veritas.Government.IN.DrivingLicenceNumber> |
| Driver Licence Number | NZ | structural check; generation | <xref:Veritas.Government.NZ.DriverLicenceNumber> |
| Driver Licence Number | IE | structural check; generation | <xref:Veritas.Government.IE.DriverLicenceNumber> |
| Licencia Nacional de Conducir | AR | structural check; generation | <xref:Veritas.Government.AR.LicenciaNacionalConducir> |
| Driving Licence Number | CN | structural check; generation | <xref:Veritas.Government.CN.DrivingLicenceNumber> |
| Driving Licence Number | UK | Luhn base36 check; generation | <xref:Veritas.Government.UK.DrivingLicenceNumber> |
| Número de permiso de conducción | ES | mod 23 check; generation | <xref:Veritas.Government.ES.NumeroPermisoConduccion> |
| South African Licence Number | ZA | Luhn checksum; generation | <xref:Veritas.Government.ZA.SouthAfricanLicenceNumber> |
| CNH | BR | mod 11 checksum; generation | <xref:Veritas.Government.BR.Cnh> |
| Japanese Driver Licence Number | JP | mod 11 checksum; generation | <xref:Veritas.Government.JP.JapaneseDriverLicenseNumber> |

### Standards & Protocols

| Identifier | Country/Region | Validation & Generation | Docs |
|------------|----------------|-------------------------|------|
| IEC Standard Number | International | format check; generation | <xref:Veritas.Standards.IecStandardNumber> |
| ISO Standard Number | International | format check; generation | <xref:Veritas.Standards.IsoStandardNumber> |
| RFC Number | International | format check; generation | <xref:Veritas.Standards.RfcNumber> |
| UL File Number | International | format check; generation | <xref:Veritas.Standards.UlFileNumber> |

### Geospatial & Infrastructure

| Identifier | Country/Region | Validation & Generation | Docs |
|------------|----------------|-------------------------|------|
| Geohash | International | format check; generation | <xref:Veritas.Geospatial.Geohash> |
| Plus Code (Open Location Code) | International | format check; generation | <xref:Veritas.Geospatial.PlusCode> |
| Flurstücknummer | DE | structural check; generation | <xref:Veritas.Geospatial.DE.Flurstuecknummer> |
| Kadastrale Aanduiding | NL | structural check; generation | <xref:Veritas.Geospatial.NL.KadastraleAanduiding> |

## Future identifiers

### Finance
- Bank Routing Numbers for additional countries
- International Securities Broker numbers

### Tax
- US state tax IDs
- EU VAT formats for remaining member states

### Energy
- US EIA plant codes

### Identity & Telecom
- OpenID Connect subject identifiers

### Logistics

Contributions for these and other identifiers are welcome!

## Validation boundaries

Validation is offline and does not establish official assignment or ownership. Generation produces synthetic test data that may coincide with assigned identifiers. The parsers hardened for issue #22 enforce a 4,096-character raw-input resource limit, including separators; generic Base58Check supports at most 1,024 payload bytes. See the README for the affected parser families.


## Corrected validation behaviour (unreleased)

The issue #22 corrections validate EIC with the ENTSO-E weighted algorithm, MPAN with prime weights, SEPA without the business code, FIGI and CUSIP using character-wise digit sums, and Codice Fiscale with its positional maps and calendar structure. IBAN checks the SWIFT national BBAN format; ISIN requires a letter prefix; MaLo requires a non-zero first digit. Validation can now reject values that earlier releases incorrectly accepted.

Plus Codes support full, short and padded syntax; generated full codes contain ten significant characters. KID Mod11 generation can return false for a body with an unrepresentable check digit. MRZ generation writes the two-digit year. CNPJ accepts the 2026 alphanumeric format alongside numeric CNPJs, and HETU accepts the 2023 century markers and rejects impossible dates. BCP 47 checks RFC 5646 well-formed syntax, including grandfathered and private-use tags, without claiming IANA registration.

Unseeded ULID, KSUID and NanoID generation uses cryptographic randomness. Seeded ULID/KSUID fixtures use their scheme epoch unless `GenerationOptions.Timestamp` is supplied. Timestamp values outside the scheme range fail explicitly. Seeded generation is for reproducible fixtures, not security tokens.

Unrestricted `Eic.TryGenerate` now returns false with zero characters written: ENTSO-E reserves allocation-related algorithm use to authorised issuing offices. SEPA's default synthetic generator now emits the documented German national format rather than arbitrary country/length combinations. Its validator checks the supported national shapes and the SEPA checksum, without checking each underlying national tax identifier or registry.

See [the issue #22 remediation notes](https://github.com/rebeccapowell/Veritas/blob/fix/22-critical-review/docs/remediation-22.md) for specification references, dependency audit results and operational follow-up.

## German market partner identifiers

<xref:Veritas.Energy.DE.MarketPartnerId> supports the BDEW electricity and DVGW gas schemes with explicit classification and synthetic generation. It validates the 13 ASCII digits, allocation-mode position and Lok-/Waggon checksum. Spaces are normalised. This checks structure and check digits, not registry allocation; GS1 GLNs are a separate scheme. See BDEW guide 1.2, sections 2 and 8.1.

## German network locations (NeLo)

`Energy.DE.Nelo` and `NeloValue` support the eleven-character Netzlokations-ID: `E`, nine ASCII alphanumeric characters and one decimal check digit. Validation normalises lower-case ASCII and spaces, rejects other punctuation and enforces a 4,096-character raw-input resource cap. Generation is deterministic with a seed and produces synthetic values that may already be assigned. Structure and check digits do not establish official allocation. The shared ASCII procedure is independently checked with BDEW's published `A1137355925` example. Reference: [BDEW guide 1.2, sections 4 and 8.2](https://www.bdew.de/media/documents/AWH_Identifikatoren-in-der-Marktkommunikation_Version.1.2.pdf).

## German network areas (NeBe)

`Energy.DE.Nebe` and `NebeValue` implement the distinct `F`-prefix Netzbereichs-ID under BDEW guide 1.2, sections 5 and 8.2. The eleven-character format shares the verified ASCII checksum with NeLo. Validation normalises ASCII case and spaces, rejects Unicode lookalikes and checksum-valid identifiers of another type, and applies a 4,096-character raw-input cap. Seeded generation is deterministic synthetic test data, with no promise of non-assignment or official allocation.

## German resource identifiers

`Energy.DE.ResourceId` parses a typed `ResourceIdValue`; its expected-type overload rejects a checksum-valid identifier of another type. Generation requires an explicit `ResourceIdType`. BDEW guide 1.2, sections 6 and 8.2 specifies:

| Prefix | Type | Enum |
| --- | --- | --- |
| A | Cluster resource, CR-ID | Cluster |
| B | Control group, SG-ID | ControlGroup |
| C | Controllable resource, SR-ID | Controllable |
| D | Technical resource, TR-ID | Technical |

All variants use eleven characters and the verified shared ASCII checksum. NeLo and NeBe are separate types. Validation normalises ASCII case and spaces, rejects other punctuation and Unicode lookalikes, and caps raw input at 4,096 characters. Synthetic, optionally seeded generation neither allocates a resource nor guarantees that its value is unassigned.

## Leitweg-ID invoice routing

`Finance.DE.LeitwegId` validates KoSIT format 2.0.2 component syntax and MOD97 check digits. `LeitwegIdValue` exposes coarse/fine addresses and check digits, preserving zeros. Generation accepts explicit components or seeded synthetic data, with an option to omit the fine address. Coarse lengths are 2,3,5,8,9 or12, using state codes01–16 or99; the optional fine component has1–30 ASCII alphanumeric characters and is canonicalised to uppercase. Required ASCII hyphens are preserved; spaces and a Peppol `0204:` prefix are rejected. Actual AGS entries, allocation and routing reachability are outside offline validation. BT-10 and a recipient's Peppol participant identifier need not contain identical values. Generated values may coincide with assigned routing IDs. Reference: [KoSIT 2.0.2](https://xeinkauf.de/app/uploads/2022/11/Leitweg-ID-Formatspezifikation-v2-0-2-1.pdf).

## Wirtschafts-Identifikationsnummer: structural support

`Tax.DE.WIdNr` intentionally exposes `TryValidateBaseStructure` for `DE` plus nine ASCII digits and `TryValidateStructure` for the full base plus `-` and a five-digit distinguishing number from 00001 to 99999. Values model the base and activity form separately and preserve leading zeros. Only ASCII prefix case is normalised; whitespace and other punctuation are rejected.

These methods do not verify the base check digit or assignment. `TryGenerateSyntheticStructure` and `TryGenerateSyntheticBaseStructure` produce seeded structural fixtures, with an explicit full-form suffix option; they do not calculate or promise valid check digits. There is deliberately no unqualified `TryValidate`/`TryGenerate` API.

WIdV section 1(2) confirms reuse of pre-existing VAT numbers for one population, and ERiC 43.3.2 section 6.3.5 gives `DE123456788-00001` as a checksum-valid example. Neither source establishes a universal checksum algorithm for all newly issued bases. VAT checksum reuse is therefore pending authoritative confirmation in issue #28. The frequently published `DE123456789-00001` is tested only as a shape illustration. Suffix syntax has no date-dependent restriction; current issuance schedules do not define the parser grammar. Sources checked 4 November 2026: [WIdV§1](https://www.gesetze-im-internet.de/widv/__1.html), [AO§139c](https://www.gesetze-im-internet.de/ao_1977/__139c.html), [ERiC primary-authored handbook mirror, page 171](https://instantview.org/data/CyberEnterprise/ERiC/ERiC-Entwicklerhandbuch.pdf).

## D-U-N-S business identifiers

`Commerce.Duns` validates structure only: exactly nine ASCII digits or the display form `12-345-6789`. It normalises the two defined hyphens, preserves leading zeros in `DunsValue` and rejects all other punctuation, whitespace and Unicode numerals. It does not enforce Luhn or an unconfirmed checksum. Successful offline validation does not establish D&B assignment, company identity, active registration or creditworthiness. There is no network dependency. Seeded generation produces reproducible synthetic nine-digit examples that may coincide with assigned numbers and are not promised safe for production enrichment calls. Reference: [D&B D-U-N-S documentation](https://docs.dnb.com/office-apps/en-US/DUNS-Number).

## MPAN distributor boundary

MPAN validation checks the 13-digit core, a non-zero-prefixed two-digit distributor field and the prime-weight checksum; it does not check current distributor allocation or meter registration. Synthetic generation uses distributor 12 with a random ten-digit reference. The older Ofgem FIT guidance quotes 10–28, while [EMRS WP25 appendix 4](https://www.emrsettlement.co.uk/document/working-practice/wp25-aggregation-rules-for-suppliers-and-cfd-generators/) also lists 29 and 30 and directs readers to Elexon for new LDSOs. The validator therefore avoids a stale upper allocation bound while rejecting 00–09; live allocation checks belong in the consuming application.

CNPJ normalisation checks raw ASCII letters/digits before case conversion, so Unicode letters such as long s cannot become valid ASCII checksum input. ASCII lower-case alphanumeric CNPJs remain supported.
