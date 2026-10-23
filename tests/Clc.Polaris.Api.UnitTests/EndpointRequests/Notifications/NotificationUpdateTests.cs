namespace Clc.Polaris.Api.UnitTests.EndpointRequests.Notifications
{
    [TestClass]
    [UnitTest]
    public sealed class NotificationUpdateTests : PapiClientUnitTestBase
    {
        [TestMethod]
        public async Task NotificationUpdateAsync_DefaultLogonIds_UsesConfiguredClientIds()
        {
            var handler = new ProtectedTokenHttpMessageHandler();
            var client = CreateProtectedClient(handler);
            client.OrganizationId = 101;
            client.UserId = 202;
            client.WorkstationId = 303;
            var updateParams = CreateValidParams();

            await client.NotificationUpdateAsync(updateParams, cancellationToken: TestContext.CancellationToken);

            var finalRequest = handler.CapturedRequests.Last(request => !request.IsStaffAuthenticationRequest);
            Assert.Contains("/protected/v1/1033/100/101/", finalRequest.Path);
            Assert.Contains("\"LogonBranchId\":101", finalRequest.Body);
            Assert.Contains("\"LogonUserId\":202", finalRequest.Body);
            Assert.Contains("\"LogonWorkstationId\":303", finalRequest.Body);
            Assert.Contains("\"ReportingOrgID\":101", finalRequest.Body);
        }

        [TestMethod]
        public async Task NotificationUpdateAsync_ExplicitLogonIds_PreservesSuppliedValues()
        {
            var handler = new ProtectedTokenHttpMessageHandler();
            var client = CreateProtectedClient(handler);
            client.OrganizationId = 101;
            client.UserId = 202;
            client.WorkstationId = 303;
            var updateParams = CreateValidParams();
            updateParams.LogonBranchId = 11;
            updateParams.LogonUserId = 22;
            updateParams.LogonWorkstationId = 33;
            updateParams.ReportingOrgID = 44;

            await client.NotificationUpdateAsync(updateParams, cancellationToken: TestContext.CancellationToken);

            var finalRequest = handler.CapturedRequests.Last(request => !request.IsStaffAuthenticationRequest);
            Assert.Contains("\"LogonBranchId\":11", finalRequest.Body);
            Assert.Contains("\"LogonUserId\":22", finalRequest.Body);
            Assert.Contains("\"LogonWorkstationId\":33", finalRequest.Body);
            Assert.Contains("\"ReportingOrgID\":44", finalRequest.Body);
        }

        [TestMethod]
        [DataRow(nameof(NotificationUpdateParams.LogonBranchId), 0)]
        [DataRow(nameof(NotificationUpdateParams.LogonUserId), -1)]
        [DataRow(nameof(NotificationUpdateParams.LogonWorkstationId), 0)]
        [DataRow(nameof(NotificationUpdateParams.ReportingOrgID), -1)]
        public async Task NotificationUpdateAsync_InvalidLogonIds_ThrowsArgumentOutOfRangeException(string propertyName, int value)
        {
            var client = CreateClient();
            var updateParams = CreateValidParams();
            SetValue(updateParams, propertyName, value);

            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () =>
                await client.NotificationUpdateAsync(updateParams, cancellationToken: TestContext.CancellationToken));
        }

        private static NotificationUpdateParams CreateValidParams()
        {
            return new NotificationUpdateParams
            {
                NotificationTypeId = 7,
                DeliveryOptionId = 3,
                DeliveryString = "555-0100",
                PatronId = 123,
                NotificationStatusId = NotificationStatus.EmailCompleted
            };
        }

        private static void SetValue(NotificationUpdateParams updateParams, string propertyName, int value)
        {
            switch (propertyName)
            {
                case nameof(NotificationUpdateParams.LogonBranchId):
                    updateParams.LogonBranchId = value;
                    break;
                case nameof(NotificationUpdateParams.LogonUserId):
                    updateParams.LogonUserId = value;
                    break;
                case nameof(NotificationUpdateParams.LogonWorkstationId):
                    updateParams.LogonWorkstationId = value;
                    break;
                case nameof(NotificationUpdateParams.ReportingOrgID):
                    updateParams.ReportingOrgID = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(propertyName), propertyName, "Unsupported NotificationUpdateParams property.");
            }
        }
    }
}
