namespace Clc.Polaris.Api.UnitTests.Methods.RequestShape
{
    [TestClass]
    [UnitTest]
    public sealed class PatronReferenceDataRequestShapeTests : PapiClientUnitTestBase
    {
        [TestMethod]
        [DataRow("languages", "/PAPIService/REST/public/v1/1033/100/101/patronlanguages")]
        [DataRow("statisticalClasses", "/PAPIService/REST/public/v1/1033/100/101/patronstatisticalclasses")]
        [DataRow("udfConfigs", "/PAPIService/REST/public/v1/1033/100/101/patronudfs")]
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
        public async Task PatronStatisticalClassesGetAsync_DeserializesRows()
        {
            var handler = new CapturingHttpMessageHandler(CreateJson(new
            {
                PAPIErrorCode = 0,
                PatronStatisticalClassesRows = new[] { new { StatisticalClassID = 1, OrganizationID = 101, Description = "Adult" } }
            }));
            var client = CreateConfiguredClient(handler);

            var response = await client.PatronStatisticalClassesGetAsync(cancellationToken: TestContext.CancellationToken);

            Assert.IsNotNull(response.Data);
            Assert.AreEqual(1, response.Data!.PatronStatisticalClassesRows[0].StatisticalClassID);
            Assert.AreEqual(101, response.Data.PatronStatisticalClassesRows[0].OrganizationID);
            Assert.AreEqual("Adult", response.Data.PatronStatisticalClassesRows[0].Description);
        }

        [TestMethod]
        public async Task PatronUdfConfigsGetAsync_DeserializesRows()
        {
            var handler = new CapturingHttpMessageHandler(CreateJson(new
            {
                PAPIErrorCode = 0,
                PatronUdfConfigsRows = new[] { new { UDFID = 1, OrganizationID = 101, Description = "Custom Field", Label = "Custom", Enabled = true } }
            }));
            var client = CreateConfiguredClient(handler);

            var response = await client.PatronUdfConfigsGetAsync(cancellationToken: TestContext.CancellationToken);

            Assert.IsNotNull(response.Data);
            Assert.AreEqual(1, response.Data!.PatronUdfConfigsRows[0].UDFID);
            Assert.AreEqual(101, response.Data.PatronUdfConfigsRows[0].OrganizationID);
            Assert.AreEqual("Custom Field", response.Data.PatronUdfConfigsRows[0].Description);
            Assert.AreEqual("Custom", response.Data.PatronUdfConfigsRows[0].Label);
            Assert.AreEqual(true, response.Data.PatronUdfConfigsRows[0].Enabled);
        }

        [TestMethod]
        public async Task PickupAreasGetAsync_DeserializesRows()
        {
            var handler = new CapturingHttpMessageHandler(CreateJson(new
            {
                PAPIErrorCode = 0,
                PickupAreasRows = new[] { new { PickupAreaID = 1, OrganizationID = 101, Description = "Main" } }
            }));
            var client = CreateConfiguredClient(handler);

            var response = await client.PickupAreasGetAsync(cancellationToken: TestContext.CancellationToken);

            Assert.IsNotNull(response.Data);
            Assert.AreEqual(1, response.Data!.PickupAreasRows[0].PickupAreaID);
            Assert.AreEqual(101, response.Data.PickupAreasRows[0].OrganizationID);
            Assert.AreEqual("Main", response.Data.PickupAreasRows[0].Description);
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
            return client;
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
    }
}
