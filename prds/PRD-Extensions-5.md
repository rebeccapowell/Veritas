# PRD-Extensions-5 --- Additional Sector Identifiers (Full Coverage)

**Goal.** Identify *all* additional sectors and identifiers not yet
implemented in Veritas, to ensure comprehensive coverage. Includes
global, regional, and country-specific codes.

**Non-Goals.** No external registry lookup; no business logic beyond
format/checksum/generation.

------------------------------------------------------------------------

## At-a-glance table (all sectors, incl. expanded driver's licences)

  -----------------------------------------------------------------------------------------------------
  sector           country code   identifier               validation strategy -- generation strategy
                                                           algorithm              
  ---------------- -------------- ------------------------ ---------------------- ---------------------
  Health / Medical --- (global)   **UDI** (Unique Device   Structural validation  Generate DI +
                                  Identifier)              per issuing agency     optional PI (serial,
                                                           (GS1, HIBCC, ICCBBA).  lot, expiry).
                                                           No universal checksum. 

  Health / Medical US             **NDC** (National Drug   10 or 11 digits (3     Generate segments
                                  Code)                    segments). Structural  with valid padding.
                                                           validation.            

  Health / Medical --- (global)   **ICD**                  Structural: letter +   Generate structurally
                                  (ICD-9/ICD-10/ICD-11     digits, optional       valid examples.
                                  codes)                   sub-codes.             

  Health / Medical US             **RxNorm Identifier**    Numeric code,          Generate numeric IDs
                                                           structural only.       in valid ranges.

  Health / Medical US             **NDC Package Code**     Extension of NDC for   Generate test values
                                                           packages.              structurally.

  Education /      --- (global)   **ROR ID** (Research     URL prefix             Generate prefix +
  Academia                        Organization Registry)   `https://ror.org/` + 9 random 9-char alnum.
                                                           alnum. Structural.     

  Education /      --- (global)   **Scopus Author ID**     Numeric, 11-digit.     Generate 11-digit
  Academia                                                 Structural only.       IDs.

  Education /      --- (global)   **ResearcherID**         Format A-####-YYYY.    Generate prefix +
  Academia                                                 Structural.            numeric seq + year.

  Education /      --- (global)   **GRID ID**              Grid.+7 digits.+1      Generate test values
  Academia                                                 char. Structural.      with Grid prefix.

  Transportation / US             **FAA N-number**         Format: "N" + 1--5     Generate samples
  Vehicles                                                 digits/letters.        matching FAA formats.
                                                           Structural.            

  Transportation / --- (global)   **ICAO Airline Code**    3 letters. Structural. Generate random
  Vehicles                                                                        letters.

  Transportation / --- (global)   **IATA Airline Code**    2 letters/digits.      Generate codes per
  Vehicles                                                 Structural.            IATA pattern.

  Transportation / --- (global)   **Flight Number**        Airline code + 1--4    Generate airline
  Vehicles                                                 digits. Structural.    prefix + digits.

  Transportation / --- (global)   **Train UIC Number**     12-digit with Mod 11   Generate 11 digits,
  Vehicles                                                 checksum.              compute check digit.

  Transportation / --- (global)   **IMO Call Sign**        3--7 alphanumeric.     Generate prefix +
  Vehicles                                                 Structural.            alphanumeric core.

  Geospatial /     --- (global)   **Geohash**              Base32 encoding,       Generate random
  Infrastructure                                           length 1--12.          geohashes.

  Geospatial /     --- (global)   **Plus Code (Open        Alphabet 20 chars,     Generate codes at
  Infrastructure                  Location Code)**         includes "+"           various precisions.
                                                           separator.             

  Geospatial /     DE             **Flurstücknummer**      German land parcel ID. Generate structural
  Infrastructure                                           Structural.            samples.

  Geospatial /     NL             **Kadastrale             Dutch parcel ID.       Generate examples
  Infrastructure                  Aanduiding**             Structural.            with
                                                                                  gemeente/kadastrale
                                                                                  sectie.

  Standards /      --- (global)   **ISO Standard Number**  Format: ISO            Generate ISO prefix +
  Protocols                                                ####(:####).           number.
                                                           Structural.            

  Standards /      --- (global)   **IEC Standard Number**  Format: IEC            Generate sample
  Protocols                                                ####(:####).           codes.
                                                           Structural.            

  Standards /      --- (global)   **RFC Number**           RFC + digits.          Generate RFC + random
  Protocols                                                Structural.            number.

  Standards /      --- (global)   **UL File Number**       "E" + digits.          Generate E + 6
  Protocols                                                Structural.            digits.

  Crypto /         --- (global)   **Bitcoin Address**      Base58Check; version   Generate payload,
  Blockchain                                               byte + payload +       compute checksum.
                                                           4-byte checksum.       

  Crypto /         --- (global)   **Ethereum Address**     20-byte hex, optional  Generate random hex,
  Blockchain                                               EIP-55 checksum.       compute checksum.

  Crypto /         --- (global)   **Ethereum Tx Hash**     32-byte hex.           Generate random
  Blockchain                                               Structural.            64-char hex.

  Crypto /         --- (global)   **Chain ID**             Integer.               Generate integer IDs.
  Blockchain                                                                      

  Government /     --- (global)   **Passport Number**      Country-specific       Generate test
  Public Admin                                             structures.            passport IDs by
                                                                                  country.

  Government /     --- (global)   **Visa Number**          Country-specific.      Generate structural
  Public Admin                                             Structural.            examples.

  Government /     US             **Driver License         State-specific formats Generate random
  Public Admin                    Numbers**                (CA, NY, FL, TX, etc). per-state DLNs.
                                                           Structural validation. 

  Government /     UK             **Driving Licence        16 chars with encoded  Validate checksum;
  Public Admin                    Number**                 DOB and initials;      generate with
                                                           includes checksum      name/DOB test data.
                                                           digit.                 

  Government /     DE             **Führerscheinnummer**   10--12 chars,          Generate random
  Public Admin                                             authority code.        patterns.
                                                           Structural only.       

  Government /     FR             **Permis de Conduire**   12 digits. Structural. Generate random
  Public Admin                                                                    digits.

  Government /     IT             **Patente di guida**     10 alphanumeric.       Generate test
  Public Admin                                                                    strings.

  Government /     ES             **Número de permiso de   8 digits + Mod 23      Validate Mod 23;
  Public Admin                    conducción**             letter (like DNI).     generate with
                                                                                  computed letter.

  Government /     NL             **Rijbewijsnummer**      9 chars (2 letters + 6 Generate structural
  Public Admin                                             digits + letter).      samples.

  Government /     SE             **Körkortsnr**           12 digits              Validate via
  Public Admin                                             (Personnummer).        Personnummer rules.

  Government /     CA             **Provincial DLNs**      ON, BC, QC, etc each   Generate per-province
  Public Admin                                             with own structure.    structural IDs.

  Government /     AU             **State DLNs**           NSW, VIC, QLD, WA,     Generate per-state.
  Public Admin                                             ACT/SA each different. 

  Government /     IN             **Driving Licence        SS-RR-YYYYNNNNNNN      Generate with codes +
  Public Admin                    Number**                 (state code, RTO code, sequence.
                                                           year, sequence).       

  Government /     NZ             **Driver Licence         8 alphanumeric.        Generate random.
  Public Admin                    Number**                                        

  Government /     IE             **Irish DLN**            9 digits.              Generate numeric.
  Public Admin                                                                    

  Government /     ZA             **South African Licence  13 digits with Luhn    Validate Luhn;
  Public Admin                    Number**                 check.                 generate with valid
                                                                                  check digit.

  Government /     BR             **CNH**                  11 digits with Mod 11  Validate & generate
  Public Admin                                             checksum.              with checksum.

  Government /     MX             **Licencia de Conducir** State-specific.        Generate samples.
  Public Admin                                             Structural.            

  Government /     AR             **Licencia Nacional de   7--8 digits.           Generate numeric IDs.
  Public Admin                    Conducir**                                      

  Government /     CN             **Driving Licence        12 digits, linked to   Structural-only.
  Public Admin                    Number**                 ID.                    

  Government /     JP             **Japanese DLN**         12 digits, last digit  Validate & generate
  Public Admin                                             Mod 11 checksum.       with check digit.

  Financial        --- (global)   **FIGI**                 12-char with check     Generate per spec.
  Markets                                                  digit.                 

  Financial        --- (global)   **Other Exchange /       Structural only.       Generate samples.
  Markets                         Clearing IDs**                                  
  -----------------------------------------------------------------------------------------------------

------------------------------------------------------------------------

## Acceptance Criteria

-   Implement each identifier with static class + value type.\
-   Use country sub-namespaces for driver's licence numbers and other
    national IDs.\
-   Provide generation routines for test values where checksums are
    defined.\
-   Mirror test structure under `test/Veritas.Tests`.\
-   Update README and registry with new sectors.
