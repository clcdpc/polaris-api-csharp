using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class CollectionsGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task CollectionsGetTest()
        {
            var response = await Papi.CollectionsGetAsync();
            Assert.IsTrue(response.Data.PAPIErrorCode > 300);
            Assert.IsTrue(response.Data.CollectionsRows.Count > 300);
            Assert.AreEqual(response.Data.PAPIErrorCode, response.Data.CollectionsRows.Count);
        }
    }
}
