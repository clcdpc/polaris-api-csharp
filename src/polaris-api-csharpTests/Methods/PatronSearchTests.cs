using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronSearchTests : IntegrationTestBase
    {
        [TestMethod]
        [ProtectedReadOnlyIntegrationTest]
        public async Task PatronSearchTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.PatronSearchAsync($"PRID={Settings.PatronId}");
            Assert.IsTrue(response.Data.PAPIErrorCode == response.Data.PatronSearchRows.Count);
        }
    }
}
