namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronAccountTransactionLifecycleTests : IntegrationTestBase
    {
        [TestMethod]
        [ProtectedMutatingIntegrationTest]
        [DoNotParallelize]
        public async Task PatronAccountCreditAndDeposit_CreateRowsVisibleInAccountReadback()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var creditNote = CreateUniqueTestArtifactText("credit", maxLength: 80);
            var creditResponse = await Papi.PatronAccountCreateCreditAsync(Settings.PatronBarcode, .01, PaymentMethod.Cash, workstationId: Settings.StaffWorkstationId, userId: Settings.StaffUserId, note: creditNote, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(0, creditResponse.Data.PAPIErrorCode);

            var afterCreditAccount = await Papi.PatronAccountGetAsync(Settings.PatronBarcode, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(0, afterCreditAccount.Data.PAPIErrorCode);
            Assert.IsNotNull(afterCreditAccount.Data.PatronAccountGetRows.SingleOrDefault(row => row.FreeTextNote == creditNote && Math.Abs(row.TransactionAmount - .01) < .0001));

            var depositNote = CreateUniqueTestArtifactText("deposit", maxLength: 80);
            var depositResponse = await Papi.PatronAccountDepositCreditAsync(Settings.PatronBarcode, .01, workstationId: Settings.StaffWorkstationId, userId: Settings.StaffUserId, note: depositNote, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(0, depositResponse.Data.PAPIErrorCode);

            var afterDepositAccount = await Papi.PatronAccountGetAsync(Settings.PatronBarcode, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(0, afterDepositAccount.Data.PAPIErrorCode);
            Assert.IsNotNull(afterDepositAccount.Data.PatronAccountGetRows.SingleOrDefault(row => row.FreeTextNote == depositNote && Math.Abs(row.TransactionAmount - .01) < .0001));
        }
    }
}
