namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class PatronAccountTransactionLifecycleTests : IntegrationTestBase
    {
        [TestMethod]
        [StaffLifecycleLiveTest]
        [DoNotParallelize]
        public async Task PatronAccountCreditAndDeposit_CreateRowsVisibleInAccountReadback()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);
            RequireConfiguredStaffUser();
            RequireConfiguredStaffWorkstation();

            var creditNote = CreateUniqueTestArtifactText("credit", maxLength: 60);
            var creditResponse = await Papi.PatronAccountCreateCreditAsync(Settings.PatronBarcode, .01, PaymentMethod.Cash, workstationId: Settings.StaffWorkstationId, userId: Settings.StaffUserId, note: creditNote, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(0, creditResponse.Data.PAPIErrorCode);

            var afterCreditAccount = await Papi.PatronAccountGetAsync(Settings.PatronBarcode, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(0, afterCreditAccount.Data.PAPIErrorCode);
            AssertAccountReadbackContainsTransaction(afterCreditAccount.Data.PatronAccountGetRows, creditNote, .01, "Expected PatronAccountGetAsync to return the credit created by PatronAccountCreateCreditAsync.");

            var depositNote = CreateUniqueTestArtifactText("deposit", maxLength: 60);
            var depositResponse = await Papi.PatronAccountDepositCreditAsync(Settings.PatronBarcode, .01, workstationId: Settings.StaffWorkstationId, userId: Settings.StaffUserId, note: depositNote, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(0, depositResponse.Data.PAPIErrorCode);

            var afterDepositAccount = await Papi.PatronAccountGetAsync(Settings.PatronBarcode, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(0, afterDepositAccount.Data.PAPIErrorCode);
            AssertAccountReadbackContainsTransaction(afterDepositAccount.Data.PatronAccountGetRows, depositNote, .01, "Expected PatronAccountGetAsync to return the deposit created by PatronAccountDepositCreditAsync.");
        }

        private static void AssertAccountReadbackContainsTransaction(IEnumerable<PatronAccountGetRow> rows, string expectedNote, double expectedAbsoluteAmount, string failureMessage)
        {
            var rowArray = rows.ToArray();

            var matchingRow = rowArray.SingleOrDefault(row =>
                string.Equals(row.FreeTextNote, expectedNote, StringComparison.Ordinal)
                && Math.Abs(Math.Abs(row.TransactionAmount) - expectedAbsoluteAmount) < .0001);

            Assert.IsNotNull(
                matchingRow,
                $"{failureMessage}{Environment.NewLine}Expected note: {expectedNote}{Environment.NewLine}Expected absolute amount: {expectedAbsoluteAmount}{Environment.NewLine}Returned account rows:{Environment.NewLine}{FormatAccountRows(rowArray)}");
        }

        private static string FormatAccountRows(IEnumerable<PatronAccountGetRow> rows)
        {
            var formattedRows = rows.Select(row => $"FreeTextNote='{row.FreeTextNote}', TransactionAmount={row.TransactionAmount}").ToArray();
            return formattedRows.Length == 0 ? "(no rows returned)" : string.Join(Environment.NewLine, formattedRows);
        }
    }
}