using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronItemsOutGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task PatronItemsOutGetTest()
        {
            var response = await Papi.PatronItemsOutGetAsync(Settings.PatronBarcode, password: Settings.PatronPin, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(response.Data.PatronItemsOutGetRows.Count, response.Data.PAPIErrorCode);
            Assert.IsNotEmpty(response.Data.PatronItemsOutGetRows);
        }
    }
}
