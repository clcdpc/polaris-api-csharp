namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronAccountGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task PatronAccountGetTest()
        {
            var response = await Papi.PatronAccountGetAsync(Settings.PatronBarcode, Settings.PatronPin, TestContext.CancellationToken);
            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.IsNotEmpty(response.Data.PatronAccountGetRows);
        }
    }
}
