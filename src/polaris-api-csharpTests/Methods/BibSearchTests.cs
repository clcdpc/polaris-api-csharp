using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class BibSearchTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task BibSearchTest()
        {
            var response = await Papi.BibSearchAsync(new BibSearchOptions { Term = "dogs", PageSize = 10 });
            Assert.IsTrue(response.Data.PAPIErrorCode == 10);
            Assert.IsTrue(response.Data.WordList == "dogs ");
            Assert.IsTrue(response.Data.TotalRecordsFound > 10000);
        }
    }
}
