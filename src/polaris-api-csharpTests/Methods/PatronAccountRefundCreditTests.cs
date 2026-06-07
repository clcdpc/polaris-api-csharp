using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronAccountRefundCreditTests : IntegrationTestBase
    {
        [TestMethod]
        [ProtectedMutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task PatronAccountRefundCreditTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.PatronAccountRefundCreditAsync(Settings.PatronBarcode, 999999.99, note: CreateUniqueTestArtifactText(maxLength: 80));
            Assert.IsTrue(response.Data.PAPIErrorCode == -3606);
        }
    }
}
