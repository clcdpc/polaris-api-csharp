using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class CollectionsGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationCategory]
        public async Task CollectionsGetTest()
        {
            var response = await Papi.CollectionsGetAsync();
            Assert.IsTrue(response.Data.PAPIErrorCode > 300);
            Assert.IsTrue(response.Data.CollectionsRows.Count > 300);
            Assert.AreEqual(response.Data.PAPIErrorCode, response.Data.CollectionsRows.Count);
        }
    }
}
