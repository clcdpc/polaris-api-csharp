using System;
using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class HoldRequestReplyTests : IntegrationTestBase
    {
        [TestMethod]
        [MutatingIntegrationTest]
        [DoNotParallelize]
        public async Task HoldRequestReplyTest()
        {
            var hold = new HoldRequestCreateResult
            {
                RequestGuid = new Guid(),
                TxnGroupQualifier = "test",
                TxnQualifier = "test"
            };
            var response = await Papi.HoldRequestReplyAsync(hold, 7, HoldRequestReplyAnswer.Yes, HoldRequestReplyState.AcceptEvenWithExistingHolds);
            Assert.IsTrue(response.Data.PAPIErrorCode == -4101);
        }
    }
}
