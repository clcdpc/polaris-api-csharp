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
            var response = await Papi.ApiVersionGetAsync();
            Assert.AreEqual(response.Data.PAPIErrorCode, 0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.ToString()));
        }
    }
}
