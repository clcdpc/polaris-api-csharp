namespace Clc.Polaris.Api.UnitTests.PapiClientBehavior
{
    [TestClass]
    [UnitTest]
    public sealed class ItemRenewValidationTests : PapiClientUnitTestBase
    {
        private const int TestOrganizationId = 73;
        private const int TestUserId = 11;
        private const int TestWorkstationId = 22;
        private const string TestBarcode = "PAT123456";

        [TestMethod]
        public async Task ItemRenewAsync_NullRenewOptions_UsesConfiguredContextInRouteAndBody()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);

            await client.ItemRenewAsync(TestBarcode, itemId: 12345, cancellationToken: TestContext.CancellationToken);

            AssertLastRequestPathContains(handler, $"/public/v1/1033/100/73/patron/{TestBarcode}/itemsout/12345");
            AssertLastRequestBodyJsonPropertyValue(handler, nameof(ItemRenewOptions.LogonBranchID), 73);
            AssertLastRequestBodyJsonPropertyValue(handler, nameof(ItemRenewOptions.LogonUserID), 11);
            AssertLastRequestBodyJsonPropertyValue(handler, nameof(ItemRenewOptions.LogonWorkstationID), 22);
        }


        [TestMethod]
        public async Task ItemRenewAsync_EmptyRenewOptions_UsesConfiguredContextInBody()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);

            await client.ItemRenewAsync(TestBarcode, itemId: 12345, renewOptions: new ItemRenewOptions(), cancellationToken: TestContext.CancellationToken);

            AssertLastRequestBodyJsonPropertyValue(handler, nameof(ItemRenewOptions.LogonBranchID), 73);
            AssertLastRequestBodyJsonPropertyValue(handler, nameof(ItemRenewOptions.LogonUserID), 11);
            AssertLastRequestBodyJsonPropertyValue(handler, nameof(ItemRenewOptions.LogonWorkstationID), 22);
        }

        [TestMethod]
        public async Task ItemRenewAsync_ExplicitRenewOptions_PreservesSuppliedValues()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateConfiguredClient(handler);
            var renewOptions = new ItemRenewOptions(17, 18, 19);

            await client.ItemRenewAsync(TestBarcode, itemId: 12345, renewOptions: renewOptions, cancellationToken: TestContext.CancellationToken);

            AssertLastRequestBodyJsonPropertyValue(handler, nameof(ItemRenewOptions.LogonBranchID), 17);
            AssertLastRequestBodyJsonPropertyValue(handler, nameof(ItemRenewOptions.LogonUserID), 18);
            AssertLastRequestBodyJsonPropertyValue(handler, nameof(ItemRenewOptions.LogonWorkstationID), 19);
        }

        [TestMethod]
        [DataRow(-1)]
        public async Task ItemRenewAsync_InvalidItemId_ThrowsArgumentOutOfRangeException(int itemId)
        {
            var client = CreateClient();

            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () =>
                await client.ItemRenewAsync(TestBarcode, itemId: itemId, cancellationToken: TestContext.CancellationToken));
        }

        [TestMethod]
        [DataRow(nameof(ItemRenewOptions.LogonBranchID), 0)]
        [DataRow(nameof(ItemRenewOptions.LogonBranchID), -1)]
        [DataRow(nameof(ItemRenewOptions.LogonUserID), 0)]
        [DataRow(nameof(ItemRenewOptions.LogonUserID), -1)]
        [DataRow(nameof(ItemRenewOptions.LogonWorkstationID), 0)]
        [DataRow(nameof(ItemRenewOptions.LogonWorkstationID), -1)]
        public async Task ItemRenewAsync_InvalidRenewOptionId_ThrowsArgumentOutOfRangeException(string propertyName, int value)
        {
            var client = CreateClient();
            var renewOptions = new ItemRenewOptions(1, 1, 1);

            SetRenewOptionValue(renewOptions, propertyName, value);

            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () =>
                await client.ItemRenewAsync(TestBarcode, itemId: 12345, renewOptions: renewOptions, cancellationToken: TestContext.CancellationToken));
        }

        private static PapiClient CreateConfiguredClient(CapturingHttpMessageHandler handler)
        {
            var client = CreateClient(handler);
            client.OrganizationId = TestOrganizationId;
            client.UserId = TestUserId;
            client.WorkstationId = TestWorkstationId;

            return client;
        }

        private static void SetRenewOptionValue(ItemRenewOptions renewOptions, string propertyName, int value)
        {
            switch (propertyName)
            {
                case nameof(ItemRenewOptions.LogonBranchID):
                    renewOptions.LogonBranchID = value;
                    break;

                case nameof(ItemRenewOptions.LogonUserID):
                    renewOptions.LogonUserID = value;
                    break;

                case nameof(ItemRenewOptions.LogonWorkstationID):
                    renewOptions.LogonWorkstationID = value;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(propertyName), propertyName, "Unsupported ItemRenewOptions property.");
            }
        }
    }
}
