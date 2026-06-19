namespace Clc.Polaris.Api.LiveIntegrationTests.ReadOnly.Patron
{
    [TestClass]
    public sealed class PatronUdfConfigsGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task PatronUdfConfigsGetAsync_ReturnsRows()
        {
            var response = await Papi.PatronUdfConfigsGetAsync(cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(response.Data.PatronUdfConfigsRows.Count, response.Data.PAPIErrorCode);
            Assert.IsNotEmpty(response.Data.PatronUdfConfigsRows);
        }
    }
}
