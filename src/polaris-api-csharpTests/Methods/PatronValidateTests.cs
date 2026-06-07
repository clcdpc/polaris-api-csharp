using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronValidateTests : IntegrationTestBase
    {
        [TestMethod]
        [ReadOnlyIntegrationCategory]
        public async Task PatronValidateTest()
        {
            var response = await Papi.PatronValidateAsync(Settings.PatronBarcode, Settings.PatronPin);
            Assert.IsTrue(response.Data.PatronID == Settings.PatronId);
        }
    }
}
