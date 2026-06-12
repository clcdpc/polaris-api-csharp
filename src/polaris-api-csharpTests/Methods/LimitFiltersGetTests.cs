using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class LimitFiltersGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationTest]
        public async Task LimitFiltersGetTest()
        {
            var response = (await Papi.LimitFiltersGetAsync(cancellationToken: TestContext.CancellationToken)).Data;
            Assert.HasCount(response.PAPIErrorCode, response.LimitFiltersRows);
        }

        
    }
}
