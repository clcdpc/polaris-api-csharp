using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

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
