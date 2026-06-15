namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class PatronLanguagesGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task PatronLanguagesGetAsync_ReturnsRows()
        {
            var response = await Papi.PatronLanguagesGetAsync(cancellationToken: TestContext.CancellationToken);

            Assert.AreEqual(response.Data.PatronLanguagesRows.Count, response.Data.PAPIErrorCode);
            Assert.IsNotEmpty(response.Data.PatronLanguagesRows);
        }
    }
}
