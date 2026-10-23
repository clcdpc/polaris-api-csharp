namespace Clc.Polaris.Api.UnitTests.EndpointRequests.Patron
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
        [DataRow(nameof(PatronRegistrationParams.LogonBranchID), 0)]
        [DataRow(nameof(PatronRegistrationParams.LogonBranchID), -1)]
        [DataRow(nameof(PatronRegistrationParams.LogonUserID), 0)]
        [DataRow(nameof(PatronRegistrationParams.LogonUserID), -1)]
        [DataRow(nameof(PatronRegistrationParams.LogonWorkstationID), 0)]
        [DataRow(nameof(PatronRegistrationParams.LogonWorkstationID), -1)]
        public async Task PatronRegistrationCreateAsync_InvalidLogonIds_ThrowsArgumentOutOfRangeException(string propertyName, int value)
        {
            var client = CreateClient();
            var registration = CreateValidParams();

            SetValue(registration, propertyName, value);

            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () =>
                await client.PatronRegistrationCreateAsync(registration, cancellationToken: TestContext.CancellationToken));
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
        [DataRow(0, null, null)]
        [DataRow(-1, null, null)]
        [DataRow(null, 0, null)]
        [DataRow(null, -1, null)]
        [DataRow(null, null, 0)]
        [DataRow(null, null, -1)]
        public void PatronRegistrationParams_ConstructorInvalidLogonIds_ThrowsArgumentOutOfRangeException(int? logonBranchId, int? logonUserId, int? logonWorkstationId)
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
                new PatronRegistrationParams(4, "Test", "Patron", logonBranchId, logonUserId, logonWorkstationId));
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

        [TestMethod]
        public async Task PatronRegistrationCreateV2Async_ExplicitLogonIds_PreservesSuppliedValues()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);
            var registration = CreateValidData();
            registration.LogonBranchID = 11;
            registration.LogonUserID = 22;
            registration.LogonWorkstationID = 33;

            await client.PatronRegistrationCreateV2Async(registration, cancellationToken: TestContext.CancellationToken);

            AssertLastRequestBodyJsonPropertyValue(handler, nameof(PatronRegistrationData.LogonBranchID), 11);
            AssertLastRequestBodyJsonPropertyValue(handler, nameof(PatronRegistrationData.LogonUserID), 22);
            AssertLastRequestBodyJsonPropertyValue(handler, nameof(PatronRegistrationData.LogonWorkstationID), 33);
        }

        [TestMethod]
        [DataRow(nameof(PatronRegistrationData.LogonBranchID), 0)]
        [DataRow(nameof(PatronRegistrationData.LogonBranchID), -1)]
        [DataRow(nameof(PatronRegistrationData.LogonUserID), 0)]
        [DataRow(nameof(PatronRegistrationData.LogonUserID), -1)]
        [DataRow(nameof(PatronRegistrationData.LogonWorkstationID), 0)]
        [DataRow(nameof(PatronRegistrationData.LogonWorkstationID), -1)]
        public async Task PatronRegistrationCreateV2Async_InvalidLogonIds_ThrowsArgumentOutOfRangeException(string propertyName, int value)
        {
            var client = CreateClient();
            var registration = CreateValidData();

            SetValue(registration, propertyName, value);

            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () =>
                await client.PatronRegistrationCreateV2Async(registration, cancellationToken: TestContext.CancellationToken));
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

        private static void SetValue(PatronRegistrationParams registration, string propertyName, int value)
        {
            switch (propertyName)
            {
                case nameof(PatronRegistrationParams.LogonBranchID):
                    registration.LogonBranchID = value;
                    break;

                case nameof(PatronRegistrationParams.LogonUserID):
                    registration.LogonUserID = value;
                    break;

                case nameof(PatronRegistrationParams.LogonWorkstationID):
                    registration.LogonWorkstationID = value;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(propertyName), propertyName, "Unsupported PatronRegistrationParams property.");
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