using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronCodesGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationCategory]
        public async Task PatronCodesGetTest()
        {
            var response = await Papi.PatronCodesGetAsync();
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
            Assert.IsTrue(response.Data.PatronCodesRows.Any());
        }
    }
}
