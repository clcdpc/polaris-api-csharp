using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class ApiKeyValidateTests : IntegrationTestBase
    {
        [TestMethod()]
        [ReadOnlyIntegrationCategory]
        public async Task ApiKeyValidateTest()
        {
            var response = await Papi.ApiKeyValidateAsync();
            Assert.AreEqual(response.Data.PAPIErrorCode, 0);
        }
    }
}
