using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronAccountVoidTests : IntegrationTestBase
    {
        [TestMethod]
        [ProtectedMutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task PatronAccountVoidTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.PatronAccountVoidAsync(Settings.PatronBarcode, 1234, note: CreateUniqueTestArtifactText(maxLength: 80));
            Assert.IsTrue(response.Data.PAPIErrorCode == -3606);
        }
    }
}
