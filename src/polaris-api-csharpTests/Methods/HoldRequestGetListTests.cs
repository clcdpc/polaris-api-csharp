using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class HoldRequestGetListTests : IntegrationTestBase
    {
        [TestMethod()]
        [ProtectedReadOnlyIntegrationCategory]
        public async Task HoldRequestGetListTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.HoldRequestGetListAsync(7);
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
        }
    }
}
