using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronILLRequestsGetTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationCategory]
        public async Task PatronILLRequestsGetTest()
        {
            var response = await Papi.PatronILLRequestsGetAsync(Settings.PatronBarcode, password: Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
            //Assert.IsTrue(response.Data.PatronILLRequestsGetRows.Any());
        }
    }
}
