using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class ApiVersionGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task ApiVersionGetTest()
        {
            var response = await Papi.ApiVersionGetAsync(TestContext.CancellationToken);
            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.ToString()));
        }
    }
}
