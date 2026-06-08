using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class ApiKeyValidateTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task ApiKeyValidateTest()
        {
            var response = await Papi.ApiKeyValidateAsync();
            Assert.AreEqual(response.Data.PAPIErrorCode, 0);
        }
    }
}
