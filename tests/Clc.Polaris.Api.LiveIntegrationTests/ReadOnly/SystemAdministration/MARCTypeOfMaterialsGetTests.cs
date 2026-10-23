namespace Clc.Polaris.Api.LiveIntegrationTests.ReadOnly.SystemAdministration
{
    [TestClass]
    public sealed class MARCTypeOfMaterialsGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task MARCTypeOfMaterialsGetAsyncTest()
        {
            var response = (await Papi.MARCTypeOfMaterialsGetAsync(cancellationToken: TestContext.CancellationToken)).Data;
            Assert.HasCount(response.PAPIErrorCode, response.MARCTypeOfMaterialsRows);
        }
    }
}
