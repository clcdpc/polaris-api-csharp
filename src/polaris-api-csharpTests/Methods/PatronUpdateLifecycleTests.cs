namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronUpdateLifecycleTests : IntegrationTestBase
    {
        [TestMethod]
        [MutatingIntegrationTest]
        [DoNotParallelize]
        public async Task UpdatePatronNotesData_NonBlockingNoteCanBeReadBackFromBasicData()
        {
            var note = CreateUniqueTestArtifactText("nonblocking-note", maxLength: 80);

            var updateResponse = await Papi.UpdatePatronNotesDataAsync(Settings.PatronBarcode, nonBlockingNote: note, updateMode: UpdateNoteMode.Prepend, workstationId: Settings.StaffWorkstationId, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(0, updateResponse.Data.PAPIErrorCode);

            var basicDataResponse = await Papi.PatronBasicDataGetAsync(Settings.PatronBarcode, Settings.PatronPin, notes: true, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(0, basicDataResponse.Data.PAPIErrorCode);
            Assert.IsNotNull(basicDataResponse.Data.PatronBasicData);
            Assert.IsNotNull(basicDataResponse.Data.PatronBasicData.PatronNotes);
            Assert.Contains(note, basicDataResponse.Data.PatronBasicData.PatronNotes.Value.NonBlockingStatusNotes);
        }

        [TestMethod]
        [MutatingIntegrationTest]
        [DoNotParallelize]
        public async Task PatronUpdate_EmptyParamsStillSucceedsAndLeavesBasicDataReadable()
        {
            var updateResponse = await Papi.PatronUpdateAsync(Settings.PatronBarcode, new PatronUpdateParams(), Settings.PatronPin, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(0, updateResponse.Data.PAPIErrorCode);

            var basicDataResponse = await Papi.PatronBasicDataGetAsync(Settings.PatronBarcode, Settings.PatronPin, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(0, basicDataResponse.Data.PAPIErrorCode);
            Assert.IsNotNull(basicDataResponse.Data.PatronBasicData);
        }
    }
}
