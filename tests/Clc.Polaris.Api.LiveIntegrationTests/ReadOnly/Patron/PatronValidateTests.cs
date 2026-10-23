namespace Clc.Polaris.Api.LiveIntegrationTests.ReadOnly.Patron
{
    [TestClass]
    public sealed class PatronValidateTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task PatronValidateTest()
        {
            var response = await Papi.PatronValidateAsync(Settings.PatronBarcode, Settings.PatronPin, TestContext.CancellationToken);
            Assert.AreEqual(Settings.PatronId, response.Data.PatronID);
        }
    }
}
