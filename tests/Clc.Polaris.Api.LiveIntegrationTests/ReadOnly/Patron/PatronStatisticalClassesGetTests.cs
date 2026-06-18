namespace Clc.Polaris.Api.LiveIntegrationTests.ReadOnly.Patron
{
    [TestClass]
    public sealed class PatronStatisticalClassesGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task PatronStatisticalClassesGetAsync_ReturnsRows()
        {
            var response = await Papi.PatronStatisticalClassesGetAsync(cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(response.Data.PatronStatisticalClassesRows.Count, response.Data.PAPIErrorCode);
            Assert.IsNotEmpty(response.Data.PatronStatisticalClassesRows);
        }
    }
}
