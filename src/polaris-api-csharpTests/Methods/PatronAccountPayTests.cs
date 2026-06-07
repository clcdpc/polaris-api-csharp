using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronAccountPayTests : IntegrationTestBase
    {
        [TestMethod()]
        [ProtectedMutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task PatronAccountPayTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = (await Papi.PatronAccountPayAsync(Settings.PatronBarcode, 1234, .01, PaymentMethod.Cash, note: CreateUniqueTestArtifactText(maxLength: 80))).Data;
            Assert.IsTrue(response.PAPIErrorCode == -3600);
        }
    }
}
