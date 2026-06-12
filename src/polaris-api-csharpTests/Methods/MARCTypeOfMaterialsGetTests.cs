using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class MARCTypeOfMaterialsGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task MARCTypeOfMaterialsGetAsyncTest()
        {
            var response = (await Papi.MARCTypeOfMaterialsGetAsync(cancellationToken: TestContext.CancellationToken)).Data;
            Assert.HasCount(response.PAPIErrorCode, response.MARCTypeOfMaterialsRows);
        }
    }
}
