# CLC Polaris API Helper Library

The library was recently migrated from RestSharp to .NET HttpClient and is relatively unstable. Most methods have been briefly tested and basic funtionality is working. Other bugs and edge case errors might exist but should easy to fix. Not all methods are currently supported but will eventually be added.

**Staff account credentials are only required for protected methods and overriding public methods**

## Usage

### Public Method
```csharp
var papi = new PapiClient
            {
                AccessID = "your-access-id",
                AccessKey = "your-access-secret",
                BaseUrl = "https://papi.yoursite.org"
            };

            var patron = papi.PatronCirculateBlocksGet("123000000456", "0000");
            Console.WriteLine($"Hello {patron.Data.NameFirst}");
```
<br>
### Protected Method 
```csharp
var papi = new PapiClient
            {
                AccessID = "your-access-id",
                AccessKey = "your-access-secret",
                BaseUrl = "https://papi.yoursite.org",
                StaffOverrideAccount = new PolarisUser
                {
                    Domain = "domain",
                    Username = "staff",
                    Password = "password"
                }
            };

            var value = papi.SA_GetValueByOrg(1, "orgphone1");
            Console.WriteLine(value);
```
<br>
### Public Method Override
```csharp
var papi = new PapiClient
            {
                AccessID = "your-access-id",
                AccessKey = "your-access-secret",
                BaseUrl = "https://papi.yoursite.org",
                StaffOverrideAccount = new PolarisUser
                {
                    Domain = "domain",
                    Username = "staff",
                    Password = "password"
                }
            };

            var patron = papi.PatronBasicDataGetOverride("123000000456");
            Console.WriteLine($"Hello {patron.Data.PatronBasicData.NameFirst}");
```
