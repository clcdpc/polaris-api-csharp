namespace Clc.Polaris.Api.LiveIntegrationTests.ProtectedReadOnly.Holds
{
    [TestClass]
    public sealed class HoldRequestGetListTests : IntegrationTestBase
    {
        [TestMethod]
        [ProtectedReadOnlyLiveTest]
        public async Task HoldRequestGetListTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.HoldRequestGetListAsync(7, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(0, response.Data.PAPIErrorCode);
        }
    }
}
