namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class ItemCheckOutCheckInLifecycleTests : IntegrationTestBase
    {
        [TestMethod]
        [ProtectedMutatingIntegrationTest]
        [DoNotParallelize]
        public async Task ItemCheckOutCheckInLifecycle_ChecksOutVerifiesItemsOutAndChecksIn()
        {
            var lifecycle = RequireConfiguredCheckoutCheckInLifecycleSettings();

            await CheckInConfiguredItemIfAlreadyOutToCheckoutPatronAsync(lifecycle);

            try
            {
                var checkoutResponse = await Papi.ItemCheckOutPostAsync(lifecycle.PatronBarcode, lifecycle.ItemBarcode, lifecycle.PatronPin, logonBranchId: Settings.BranchId.GetValueOrDefault(), logonUserId: Settings.StaffUserId.GetValueOrDefault(), logonWorkstationId: Settings.StaffWorkstationId.GetValueOrDefault(), cancellationToken: TestContext.CancellationToken);

                Assert.AreEqual(0, checkoutResponse.Data.PAPIErrorCode, $"Expected checkout of configured item '{lifecycle.ItemBarcode}' to configured patron '{lifecycle.PatronBarcode}' to succeed. Response: {checkoutResponse.Data}");
                Assert.IsGreaterThan(0, checkoutResponse.Data.ItemRecordID, "Checkout should return a positive ItemRecordID.");

                var checkedOutItem = await GetSingleConfiguredItemOutAsync(lifecycle, "Expected configured item to appear in PatronItemsOutGetAsync after checkout.");

                Assert.AreEqual(checkoutResponse.Data.ItemRecordID, checkedOutItem.ItemID, "PatronItemsOutGetAsync should return the same item record that checkout returned.");
                Assert.AreEqual(lifecycle.ItemBarcode, checkedOutItem.Barcode, "PatronItemsOutGetAsync should return the configured checkout item barcode.");
            }
            finally
            {
                await CheckInConfiguredItemIfAlreadyOutToCheckoutPatronAsync(lifecycle);
            }

            await AssertConfiguredItemNotOutAsync(lifecycle);
        }

        private CheckoutCheckInLifecycleTestSettings RequireConfiguredCheckoutCheckInLifecycleSettings()
        {
            var lifecycle = Settings.CheckoutCheckInLifecycle;

            RequireConfiguredStringSetting(lifecycle.PatronBarcode, $"{nameof(Settings.CheckoutCheckInLifecycle)}:{nameof(lifecycle.PatronBarcode)}");
            RequireConfiguredStringSetting(lifecycle.PatronPin, $"{nameof(Settings.CheckoutCheckInLifecycle)}:{nameof(lifecycle.PatronPin)}");
            RequireConfiguredStringSetting(lifecycle.ItemBarcode, $"{nameof(Settings.CheckoutCheckInLifecycle)}:{nameof(lifecycle.ItemBarcode)}");
            RequireConfiguredBranch();
            RequireConfiguredStaffUser();
            RequireConfiguredStaffWorkstation();
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            return lifecycle;
        }

        private static void RequireConfiguredStringSetting(string value, string settingName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Assert.Inconclusive($"Integration test setting '{settingName}' must be configured to run this live scenario.");
            }
        }

        private async Task CheckInConfiguredItemIfAlreadyOutToCheckoutPatronAsync(CheckoutCheckInLifecycleTestSettings lifecycle)
        {
            var existingItem = await GetConfiguredItemOutAsync(lifecycle);

            if (existingItem == null)
            {
                return;
            }

            var checkinResponse = await Papi.ItemCheckInPostAsync(lifecycle.ItemBarcode, logonBranchId: Settings.BranchId.GetValueOrDefault(), logonUserId: Settings.StaffUserId.GetValueOrDefault(), logonWorkstationId: Settings.StaffWorkstationId.GetValueOrDefault(), cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(0, checkinResponse.Data.PAPIErrorCode, $"Expected check-in of configured item '{lifecycle.ItemBarcode}' to succeed. Response: {checkinResponse.Data}");
        }

        private async Task<PatronItemsOutGetRow> GetSingleConfiguredItemOutAsync(CheckoutCheckInLifecycleTestSettings lifecycle, string failureMessage)
        {
            var matchingItems = await GetConfiguredItemsOutAsync(lifecycle);

            Assert.HasCount(1, matchingItems, $"{failureMessage}{Environment.NewLine}{FormatItemsOutDiagnostic(matchingItems)}");

            return matchingItems[0];
        }

        private async Task<PatronItemsOutGetRow?> GetConfiguredItemOutAsync(CheckoutCheckInLifecycleTestSettings lifecycle)
        {
            var matchingItems = await GetConfiguredItemsOutAsync(lifecycle);

            return matchingItems.SingleOrDefault();
        }

        private async Task AssertConfiguredItemNotOutAsync(CheckoutCheckInLifecycleTestSettings lifecycle)
        {
            var matchingItems = await GetConfiguredItemsOutAsync(lifecycle);

            Assert.IsEmpty(matchingItems, $"Expected configured item '{lifecycle.ItemBarcode}' to no longer appear in PatronItemsOutGetAsync after check-in.{Environment.NewLine}{FormatItemsOutDiagnostic(matchingItems)}");
        }

        private async Task<PatronItemsOutGetRow[]> GetConfiguredItemsOutAsync(CheckoutCheckInLifecycleTestSettings lifecycle)
        {
            var itemsOutResponse = await Papi.PatronItemsOutGetAsync(lifecycle.PatronBarcode, PatronItemsOutGetStatus.All, lifecycle.PatronPin, TestContext.CancellationToken);

            Assert.AreEqual(itemsOutResponse.Data.PatronItemsOutGetRows.Count, itemsOutResponse.Data.PAPIErrorCode, $"Expected PatronItemsOutGetAsync for configured checkout patron '{lifecycle.PatronBarcode}' to succeed. Response: {itemsOutResponse.Data}");

            return itemsOutResponse.Data.PatronItemsOutGetRows
                .Where(row => string.Equals(row.Barcode, lifecycle.ItemBarcode, StringComparison.OrdinalIgnoreCase))
                .ToArray();
        }

        private static string FormatItemsOutDiagnostic(PatronItemsOutGetRow[] matchingItems)
        {
            var rows = matchingItems.Select(item => $"ItemID={item.ItemID}, Barcode='{item.Barcode}', BibID={item.BibID}, Title='{item.Title}', DueDate={item.DueDate:O}").ToArray();

            return rows.Length == 0 ? "No matching checked-out items were returned." : $"Matching checked-out items:{Environment.NewLine}{string.Join(Environment.NewLine, rows)}";
        }
    }
}