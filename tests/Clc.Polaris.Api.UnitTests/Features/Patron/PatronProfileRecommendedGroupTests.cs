using System.Net;
using Clc.Polaris.Api.Models;

namespace Clc.Polaris.Api.UnitTests.Methods.RequestShape
{
    [TestClass]
    [UnitTest]
    public sealed class PatronProfileRecommendedGroupTests : PapiClientUnitTestBase
    {
        [TestMethod]
        public async Task PatronNotesGetAsync_UsesEncodedBarcodePathAndPasswordHash()
        {
            var handler = new CapturingHttpMessageHandler(CreateJson(new
            {
                PAPIErrorCode = 0,
                PatronNotes = new
                {
                    BlockingStatusNotes = "blocked",
                    NonBlockingStatusNotes = "note"
                }
            }));
            var client = CreateClient(handler);
            var barcode = "AB C/+#?=";

            var response = await client.PatronNotesGetAsync(barcode, "pin", TestContext.CancellationToken);

            Assert.AreEqual(HttpMethod.Get, GetLastRequest(handler).Method);
            Assert.AreEqual($"/PAPIService/REST/public/v1/1033/100/1/patron/{WebUtility.UrlEncode(barcode)}/notes", GetLastRequestUri(handler).AbsolutePath);
            AssertAuthorizationHashesSentUri(GetLastRequest(handler), "pin");
            Assert.AreEqual("blocked", response.Data?.PatronNotes?.BlockingStatusNotes);
            Assert.AreEqual("note", response.Data?.PatronNotes?.NonBlockingStatusNotes);
        }

        [TestMethod]
        public async Task PatronRegistrationUpdateV2Async_UsesV2PutRouteQueryAndNormalizedLogonIds()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);
            var registration = CreateValidData();

            await client.PatronRegistrationUpdateV2Async("12345", registration, "pin", ignoresa: false, cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(HttpMethod.Put, GetLastRequest(handler).Method);
            Assert.AreEqual("/PAPIService/REST/public/v2/1033/100/101/patron/12345", GetLastRequestUri(handler).AbsolutePath);
            AssertLastRequestQueryParameter(handler, "ignoresa", "False");
            AssertAuthorizationHashesSentUri(GetLastRequest(handler), "pin");
            AssertLastRequestBodyJsonPropertyValue(handler, nameof(PatronRegistrationData.LogonBranchID), 101);
            AssertLastRequestBodyJsonPropertyValue(handler, nameof(PatronRegistrationData.LogonUserID), 202);
            AssertLastRequestBodyJsonPropertyValue(handler, nameof(PatronRegistrationData.LogonWorkstationID), 303);
        }

        [TestMethod]
        public async Task PatronRegistrationUpdateV2Async_ExplicitLogonIds_PreservesSuppliedValues()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);
            var registration = CreateValidData();
            registration.LogonBranchID = 11;
            registration.LogonUserID = 22;
            registration.LogonWorkstationID = 33;

            await client.PatronRegistrationUpdateV2Async("12345", registration, cancellationToken: TestContext.CancellationToken);

            AssertLastRequestBodyJsonPropertyValue(handler, nameof(PatronRegistrationData.LogonBranchID), 11);
            AssertLastRequestBodyJsonPropertyValue(handler, nameof(PatronRegistrationData.LogonUserID), 22);
            AssertLastRequestBodyJsonPropertyValue(handler, nameof(PatronRegistrationData.LogonWorkstationID), 33);
        }

        [TestMethod]
        [DataRow(nameof(PatronRegistrationData.LogonBranchID), 0)]
        [DataRow(nameof(PatronRegistrationData.LogonUserID), -1)]
        [DataRow(nameof(PatronRegistrationData.LogonWorkstationID), 0)]
        public async Task PatronRegistrationUpdateV2Async_InvalidLogonIds_ThrowsArgumentOutOfRangeException(string propertyName, int value)
        {
            var client = CreateClient();
            var registration = CreateValidData();
            SetValue(registration, propertyName, value);

            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () =>
                await client.PatronRegistrationUpdateV2Async("12345", registration, cancellationToken: TestContext.CancellationToken));
        }

        [TestMethod]
        [DataRow("languages", "/PAPIService/REST/public/v1/1033/100/101/patronlanguages")]
        [DataRow("statisticalClasses", "/PAPIService/REST/public/v1/1033/100/101/patronstatisticalclasses")]
        [DataRow("udfConfigs", "/PAPIService/REST/public/v1/1033/100/101/patronudfconfigs")]
        [DataRow("pickupAreas", "/PAPIService/REST/public/v1/1033/100/101/pickupareas")]
        public async Task ReferenceDataMethods_DefaultOrganizationId_UseExpectedGetRoutes(string methodName, string expectedPath)
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);

            await ExecuteReferenceDataMethodAsync(client, methodName, null);

            Assert.AreEqual(HttpMethod.Get, GetLastRequest(handler).Method);
            Assert.AreEqual(expectedPath, GetLastRequestUri(handler).AbsolutePath);
        }

        [TestMethod]
        public async Task PatronLanguagesGetAsync_DeserializesRows()
        {
            var handler = new CapturingHttpMessageHandler(CreateJson(new
            {
                PAPIErrorCode = 0,
                PatronLanguagesRows = new[] { new { LanguageID = 1, Description = "English" } }
            }));
            var client = CreateConfiguredClient(handler);

            var response = await client.PatronLanguagesGetAsync(cancellationToken: TestContext.CancellationToken);

            Assert.IsNotNull(response.Data);
            Assert.AreEqual(1, response.Data!.PatronLanguagesRows[0].LanguageID);
            Assert.AreEqual("English", response.Data.PatronLanguagesRows[0].Description);
        }

        [TestMethod]
        public async Task ReferenceDataMethods_BlockStaffOverride()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);
            client.AllowStaffOverrideRequests = true;
            client.StaffOverrideAccount = CreateStaffUser();

            await client.PickupAreasGetAsync(cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(1, handler.RequestCount);
            Assert.AreEqual("/PAPIService/REST/public/v1/1033/100/101/pickupareas", GetLastRequestUri(handler).AbsolutePath);
        }

        private static PapiClient CreateConfiguredClient(CapturingHttpMessageHandler handler)
        {
            var client = CreateClient(handler);
            client.OrganizationId = 101;
            client.UserId = 202;
            client.WorkstationId = 303;
            return client;
        }

        private static PatronRegistrationData CreateValidData()
        {
            return new PatronRegistrationData
            {
                PatronBranchID = 4,
                NameFirst = "Test",
                NameLast = "Patron"
            };
        }

        private async Task ExecuteReferenceDataMethodAsync(PapiClient client, string methodName, int? organizationId)
        {
            switch (methodName)
            {
                case "languages":
                    await client.PatronLanguagesGetAsync(organizationId, TestContext.CancellationToken);
                    break;
                case "statisticalClasses":
                    await client.PatronStatisticalClassesGetAsync(organizationId, TestContext.CancellationToken);
                    break;
                case "udfConfigs":
                    await client.PatronUdfConfigsGetAsync(organizationId, TestContext.CancellationToken);
                    break;
                case "pickupAreas":
                    await client.PickupAreasGetAsync(organizationId, TestContext.CancellationToken);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(methodName), methodName, "Unsupported reference data method.");
            }
        }

        private static void SetValue(PatronRegistrationData registration, string propertyName, int value)
        {
            switch (propertyName)
            {
                case nameof(PatronRegistrationData.LogonBranchID):
                    registration.LogonBranchID = value;
                    break;
                case nameof(PatronRegistrationData.LogonUserID):
                    registration.LogonUserID = value;
                    break;
                case nameof(PatronRegistrationData.LogonWorkstationID):
                    registration.LogonWorkstationID = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(propertyName), propertyName, "Unsupported PatronRegistrationData property.");
            }
        }
    }
}
