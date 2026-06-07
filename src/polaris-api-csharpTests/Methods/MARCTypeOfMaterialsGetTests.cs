using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class MARCTypeOfMaterialsGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationCategory]
        public async Task MARCTypeOfMaterialsGetAsyncTest()
        {
            var response = (await Papi.MARCTypeOfMaterialsGetAsync()).Data;
            Assert.IsTrue(response.MARCTypeOfMaterialsRows.Count() == response.PAPIErrorCode);
        }
    }
}
