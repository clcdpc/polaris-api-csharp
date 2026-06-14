namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class HoldRequestReplyTests : IntegrationTestBase
    {
        [TestMethod]
        [MutatingLiveTest]
        [DoNotParallelize]
        public async Task HoldRequestReplyTest()
        {
            var hold = new HoldRequestCreateResult
            {
                RequestGuid = new Guid(),
                TxnGroupQualifier = "test",
                TxnQualifier = "test"
            };
            var response = await Papi.HoldRequestReplyAsync(hold, 7, HoldRequestReplyAnswer.Yes, HoldRequestReplyState.AcceptEvenWithExistingHolds, TestContext.CancellationToken);
            Assert.AreEqual(-4101, response.Data.PAPIErrorCode);
        }
    }
}
