using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class BibGetTests : IntegrationTestBase
    {
        private const int BibId = 478907;

        [TestMethod]
        [ReadOnlyIntegrationCategory]
        public async Task BibGetTest()
        {
            var response = await Papi.BibGetAsync(478907);
            Assert.AreEqual(response.Data.PAPIErrorCode, 0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.Title));
            Assert.IsTrue(response.Response.RequestMessage.RequestUri.ToString().Contains("100/1/bib"));
        }

        [TestMethod]
        [ReadOnlyIntegrationCategory]
        public async Task BibGetTest_PassBranchId()
        {
            var response = await Papi.BibGetAsync(BibId, 7);
            Assert.AreEqual(response.Data.PAPIErrorCode, 0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.Title));
            Assert.IsTrue(response.Response.RequestMessage.RequestUri.ToString().Contains("100/7/bib"));
        }
    }
}
