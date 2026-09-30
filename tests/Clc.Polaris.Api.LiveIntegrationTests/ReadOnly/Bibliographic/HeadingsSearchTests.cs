namespace Clc.Polaris.Api.LiveIntegrationTests.ReadOnly.Bibliographic
{
    [TestClass]
    public sealed class HeadingsSearchTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyLiveTest]
        public async Task HeadingsSearchAsync_ReturnsResponse()
        {
            var response = await Papi.HeadingsSearchAsync(HeadingSearchQualifier.AU, 5, 1, cancellationToken: TestContext.CancellationToken);

            Assert.IsNotNull(response.Data);
        }
    }
}
