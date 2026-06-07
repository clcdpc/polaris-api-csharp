using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class MARCTypeOfMaterialsGetTests : IntegrationTestBase
    {
        [TestMethod()]
        [ReadOnlyIntegrationCategory]
        public async Task MARCTypeOfMaterialsGetAsyncTest()
        {
            var response = (await Papi.MARCTypeOfMaterialsGetAsync()).Data;
            Assert.IsTrue(response.MARCTypeOfMaterialsRows.Count() == response.PAPIErrorCode);
        }
    }
}
