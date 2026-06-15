using Clc.Polaris.Api.Models;

namespace Clc.Polaris.Api.UnitTests.Methods.RequestShape
{
    [TestClass]
    [UnitTest]
    public sealed class PatronRegistrationUpdateV2Tests : PapiClientUnitTestBase
    {
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
