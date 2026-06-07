using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class ItemStatusesGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationCategory]
        public async Task ItemStatusesGetAsyncTest()
        {
            var response = await Papi.ItemStatusesGetAsync(7);
            Assert.IsTrue(response.Data.ItemStatusesRows.Count() == response.Data.PAPIErrorCode);
        }
    }
}
