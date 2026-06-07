using System.Linq;
using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class CreatePatronBlocksTests : IntegrationTestBase
    {
        [TestMethod]
        [ProtectedMutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task CreatePatronBlocksTest_FreeTextBlock()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var blockText = CreateUniqueTestArtifactText(Settings.FreeTextBlock, maxLength: 80);
            var response = await Papi.CreatePatronBlocksAsync(Settings.PatronBarcode, BlockType.FreeText, blockText);
            Assert.IsTrue(new[] { 0, -3507 }.Contains(response.Data.PAPIErrorCode));
        }

        [TestMethod]
        [ProtectedMutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task CreatePatronBlocksTest_SystemBlock()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.CreatePatronBlocksAsync(Settings.PatronBarcode, BlockType.System, "128");
            Assert.IsTrue(new[] { 0, -3507 }.Contains(response.Data.PAPIErrorCode));
        }

        [TestMethod]
        [ProtectedMutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task CreatePatronBlocksTest_LibraryAssignedBlock()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.CreatePatronBlocksAsync(Settings.PatronBarcode, BlockType.LibraryAssigned, "1");
            Assert.IsTrue(new[] { 0, -3507 }.Contains(response.Data.PAPIErrorCode));
        }
    }
}
