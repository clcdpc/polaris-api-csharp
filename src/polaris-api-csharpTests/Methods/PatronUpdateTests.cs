namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronUpdateTests : IntegrationTestBase
    {
        [TestMethod]
        [MutatingIntegrationTest]
        [DoNotParallelize]
        public async Task PatronUpdateTest()
        {
            var response = await Papi.PatronUpdateAsync(Settings.PatronBarcode, new PatronUpdateParams(), Settings.PatronPin, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(0, response.Data.PAPIErrorCode);
        }
    }
}
