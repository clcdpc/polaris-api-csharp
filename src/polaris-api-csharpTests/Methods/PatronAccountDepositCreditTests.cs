using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronAccountDepositCreditTests : IntegrationTestBase
    {
        [TestMethod]
        [ProtectedMutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task PatronAccountDepositCreditTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.PatronAccountDepositCreditAsync(Settings.PatronBarcode, .01, note: CreateUniqueTestArtifactText(maxLength: 80));
            Assert.IsTrue(response.Data.PAPIErrorCode == 0);
        }
    }
}
