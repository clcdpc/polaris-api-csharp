using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class HoldRequestReactivateTests : IntegrationTestBase
    {
        [TestMethod]
        [MutatingIntegrationTest]
        [DoNotParallelize]
        public async Task HoldRequestReactivateTest()
        {
            var response = await Papi.HoldRequestReactivateAsync(Settings.PatronBarcode, Settings.PatronPin, 1234, DateTime.Now);
            Assert.IsTrue(response.Data.PAPIErrorCode == -4201);
        }
    }
}
