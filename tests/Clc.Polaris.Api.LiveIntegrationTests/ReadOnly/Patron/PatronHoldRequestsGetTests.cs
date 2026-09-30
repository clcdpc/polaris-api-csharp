namespace Clc.Polaris.Api.LiveIntegrationTests.ReadOnly.Patron
{
    [TestClass]
    public sealed class PatronHoldRequestsGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task PatronHoldRequestsGetTest()
        {
            var response = await Papi.PatronHoldRequestsGetAsync(Settings.PatronBarcode, PatronHoldStatus.all, Settings.PatronPin, TestContext.CancellationToken);
            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.IsNotEmpty(response.Data.PatronHoldRequestsGetRows);
        }
    }
}
