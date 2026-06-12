using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class HoldRequestSuspendTests : IntegrationTestBase
    {
        [TestMethod]
        [MutatingIntegrationTest]
        [DoNotParallelize]
        public async Task HoldRequestSuspendTest()
        {
            var response = await Papi.HoldRequestSuspendAsync(Settings.PatronBarcode, 1234, DateTime.Now, Settings.PatronPin, cancellationToken: TestContext.CancellationToken);
            Assert.AreEqual(-4201, response.Data.PAPIErrorCode);
        }
    }
}
