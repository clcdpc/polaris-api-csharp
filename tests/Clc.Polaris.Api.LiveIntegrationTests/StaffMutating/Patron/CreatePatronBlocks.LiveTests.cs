namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class CreatePatronBlocksTests : IntegrationTestBase
    {
        [TestMethod]
        [ProtectedMutatingLiveTest]
        [DoNotParallelize]
        public async Task CreatePatronBlocksTest_FreeTextBlock()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var blockText = CreateUniqueTestArtifactText(Settings.FreeTextBlock, maxLength: 80);
            var response = await Papi.CreatePatronBlocksAsync(Settings.PatronBarcode, BlockType.FreeText, blockText, cancellationToken: TestContext.CancellationToken);
            Assert.Contains(response.Data.PAPIErrorCode, [0, -3507]);
        }

        [TestMethod]
        [ProtectedMutatingLiveTest]
        [DoNotParallelize]
        public async Task CreatePatronBlocksTest_SystemBlock()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.CreatePatronBlocksAsync(Settings.PatronBarcode, BlockType.System, "128", cancellationToken: TestContext.CancellationToken);
            Assert.Contains(response.Data.PAPIErrorCode, [0, -3507]);
        }

        [TestMethod]
        [ProtectedMutatingLiveTest]
        [DoNotParallelize]
        public async Task CreatePatronBlocksTest_LibraryAssignedBlock()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.CreatePatronBlocksAsync(Settings.PatronBarcode, BlockType.LibraryAssigned, "1", cancellationToken: TestContext.CancellationToken);
            Assert.Contains(response.Data.PAPIErrorCode, [0, -3507]);
        }
    }
}
