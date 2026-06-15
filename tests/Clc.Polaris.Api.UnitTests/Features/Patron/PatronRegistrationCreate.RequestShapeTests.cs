namespace Clc.Polaris.Api.UnitTests.Methods.RequestShape
{
    [TestClass]
    [UnitTest]
    public sealed class PatronRegistrationCreateTests : PapiClientUnitTestBase
    {
        [TestMethod]
        public async Task PatronRegistrationCreateAsync_DefaultLogonIds_UsesConfiguredClientIds()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);
            var registration = CreateValidParams();

            await client.PatronRegistrationCreateAsync(registration, cancellationToken: TestContext.CancellationToken);

            AssertLastRequestBodyJsonPropertyValue(handler, nameof(PatronRegistrationParams.LogonBranchID), 101);
            AssertLastRequestBodyJsonPropertyValue(handler, nameof(PatronRegistrationParams.LogonUserID), 202);
            AssertLastRequestBodyJsonPropertyValue(handler, nameof(PatronRegistrationParams.LogonWorkstationID), 303);
        }

        [TestMethod]
        public async Task PatronRegistrationCreateAsync_ExplicitLogonIds_PreservesSuppliedValues()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);
            var registration = CreateValidParams();
            registration.LogonBranchID = 11;
            registration.LogonUserID = 22;
            registration.LogonWorkstationID = 33;

            await client.PatronRegistrationCreateAsync(registration, cancellationToken: TestContext.CancellationToken);

            AssertLastRequestBodyJsonPropertyValue(handler, nameof(PatronRegistrationParams.LogonBranchID), 11);
            AssertLastRequestBodyJsonPropertyValue(handler, nameof(PatronRegistrationParams.LogonUserID), 22);
            AssertLastRequestBodyJsonPropertyValue(handler, nameof(PatronRegistrationParams.LogonWorkstationID), 33);
        }

        [TestMethod]
        public void PatronRegistrationParams_ConstructorWithoutLogonIds_LeavesLogonIdsUnset()
        {
            var registration = new PatronRegistrationParams(4, "Test", "Patron");

            Assert.IsNull(registration.LogonBranchID);
            Assert.IsNull(registration.LogonUserID);
            Assert.IsNull(registration.LogonWorkstationID);
        }

        [TestMethod]
        public async Task PatronRegistrationCreateV2Async_DefaultLogonIds_UsesConfiguredClientIds()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);
            var registration = CreateValidData();

            await client.PatronRegistrationCreateV2Async(registration, cancellationToken: TestContext.CancellationToken);

            AssertLastRequestBodyJsonPropertyValue(handler, nameof(PatronRegistrationData.LogonBranchID), 101);
            AssertLastRequestBodyJsonPropertyValue(handler, nameof(PatronRegistrationData.LogonUserID), 202);
            AssertLastRequestBodyJsonPropertyValue(handler, nameof(PatronRegistrationData.LogonWorkstationID), 303);
        }

        private static PapiClient CreateConfiguredClient(CapturingHttpMessageHandler handler)
        {
            var client = CreateClient(handler);
            client.OrganizationId = 101;
            client.UserId = 202;
            client.WorkstationId = 303;
            return client;
        }

        private static PatronRegistrationParams CreateValidParams()
        {
            return new PatronRegistrationParams
            {
                PatronBranchID = 4,
                NameFirst = "Test",
                NameLast = "Patron"
            };
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
    }
}
