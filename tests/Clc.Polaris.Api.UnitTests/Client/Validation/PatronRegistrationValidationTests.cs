namespace Clc.Polaris.Api.UnitTests.PapiClientBehavior
{
    [TestClass]
    [UnitTest]
    public sealed class PatronRegistrationValidationTests : PapiClientUnitTestBase
    {
        [TestMethod]
        public void PatronRegistrationParams_Constructor_InvalidPatronBranchId_ThrowsArgumentOutOfRangeException()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
                new PatronRegistrationParams(0, "Test", "Patron"));
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        public void PatronRegistrationParams_Constructor_InvalidFirstName_ThrowsArgumentException(string nameFirst)
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
                new PatronRegistrationParams(1, nameFirst, "Patron"));
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        public void PatronRegistrationParams_Constructor_InvalidLastName_ThrowsArgumentException(string nameLast)
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
                new PatronRegistrationParams(1, "Test", nameLast));
        }

        [TestMethod]
        [DataRow(nameof(PatronRegistrationParams.LogonBranchID), 0)]
        [DataRow(nameof(PatronRegistrationParams.LogonBranchID), -1)]
        [DataRow(nameof(PatronRegistrationParams.LogonUserID), 0)]
        [DataRow(nameof(PatronRegistrationParams.LogonUserID), -1)]
        [DataRow(nameof(PatronRegistrationParams.LogonWorkstationID), 0)]
        [DataRow(nameof(PatronRegistrationParams.LogonWorkstationID), -1)]
        [DataRow(nameof(PatronRegistrationParams.PatronBranchID), 0)]
        [DataRow(nameof(PatronRegistrationParams.PatronBranchID), -1)]
        [DataRow(nameof(PatronRegistrationParams.RequestPickupBranchID), 0)]
        [DataRow(nameof(PatronRegistrationParams.RequestPickupBranchID), -1)]
        public async Task PatronRegistrationCreateAsync_InvalidId_ThrowsArgumentOutOfRangeException(string propertyName, int value)
        {
            var client = CreateClient();
            var registration = CreateValidPatronRegistrationParams();

            SetPatronRegistrationParamsValue(registration, propertyName, value);

            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () =>
                await client.PatronRegistrationCreateAsync(registration, cancellationToken: TestContext.CancellationToken));
        }

        [TestMethod]
        public async Task PatronRegistrationCreateAsync_NullParams_ThrowsArgumentNullException()
        {
            var client = CreateClient();

            await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
                await client.PatronRegistrationCreateAsync(null!, cancellationToken: TestContext.CancellationToken));
        }

        [TestMethod]
        [DataRow(nameof(PatronRegistrationData.LogonBranchID), 0)]
        [DataRow(nameof(PatronRegistrationData.LogonBranchID), -1)]
        [DataRow(nameof(PatronRegistrationData.LogonUserID), 0)]
        [DataRow(nameof(PatronRegistrationData.LogonUserID), -1)]
        [DataRow(nameof(PatronRegistrationData.LogonWorkstationID), 0)]
        [DataRow(nameof(PatronRegistrationData.LogonWorkstationID), -1)]
        [DataRow(nameof(PatronRegistrationData.PatronBranchID), 0)]
        [DataRow(nameof(PatronRegistrationData.PatronBranchID), -1)]
        [DataRow(nameof(PatronRegistrationData.RequestPickupBranchID), 0)]
        [DataRow(nameof(PatronRegistrationData.RequestPickupBranchID), -1)]
        public async Task PatronRegistrationCreateV2Async_InvalidId_ThrowsArgumentOutOfRangeException(string propertyName, int value)
        {
            var client = CreateClient();
            var registration = CreateValidPatronRegistrationData();

            SetPatronRegistrationDataValue(registration, propertyName, value);

            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () =>
                await client.PatronRegistrationCreateV2Async(registration, cancellationToken: TestContext.CancellationToken));
        }

        [TestMethod]
        public async Task PatronRegistrationCreateV2Async_NullParams_ThrowsArgumentNullException()
        {
            var client = CreateClient();

            await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
                await client.PatronRegistrationCreateV2Async(null!, cancellationToken: TestContext.CancellationToken));
        }

        private static PatronRegistrationParams CreateValidPatronRegistrationParams()
        {
            return new PatronRegistrationParams
            {
                LogonBranchID = 1,
                LogonUserID = 2,
                LogonWorkstationID = 3,
                PatronBranchID = 4,
                NameFirst = "Test",
                NameLast = "Patron"
            };
        }

        private static PatronRegistrationData CreateValidPatronRegistrationData()
        {
            return new PatronRegistrationData
            {
                LogonBranchID = 1,
                LogonUserID = 2,
                LogonWorkstationID = 3,
                PatronBranchID = 4,
                NameFirst = "Test",
                NameLast = "Patron"
            };
        }

        private static void SetPatronRegistrationParamsValue(PatronRegistrationParams registration, string propertyName, int value)
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

                case nameof(PatronRegistrationParams.PatronBranchID):
                    registration.PatronBranchID = value;
                    break;

                case nameof(PatronRegistrationParams.RequestPickupBranchID):
                    registration.RequestPickupBranchID = value;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(propertyName), propertyName, "Unsupported PatronRegistrationParams property.");
            }
        }

        private static void SetPatronRegistrationDataValue(PatronRegistrationData registration, string propertyName, int value)
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

                case nameof(PatronRegistrationData.PatronBranchID):
                    registration.PatronBranchID = value;
                    break;

                case nameof(PatronRegistrationData.RequestPickupBranchID):
                    registration.RequestPickupBranchID = value;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(propertyName), propertyName, "Unsupported PatronRegistrationData property.");
            }
        }
    }
}
