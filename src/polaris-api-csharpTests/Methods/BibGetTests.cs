using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class BibGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task BibGetTest()
        {
            var response = await Papi.BibGetAsync(478907);
            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.Title));
            Assert.IsTrue(response.Response.RequestMessage.RequestUri.ToString().Contains("100/1/bib"));
        }

        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task BibGetTest_PassBranchId()
        {
            var response = await Papi.BibGetAsync(478907, 7);
            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.Title));
            Assert.IsTrue(response.Response.RequestMessage.RequestUri.ToString().Contains("100/7/bib"));
        }
    }
}
