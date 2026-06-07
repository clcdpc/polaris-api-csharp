using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class ApiVersionGetTests : IntegrationTestBase
    {
        [TestMethod()]
        [ReadOnlyIntegrationCategory]
        public async Task ApiVersionGetTest()
        {
            var response = await Papi.ApiVersionGetAsync();
            Assert.AreEqual(response.Data.PAPIErrorCode, 0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.ToString()));
        }
    }
}
