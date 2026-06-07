using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class HoldRequestSuspendTests : IntegrationTestBase
    {
        [TestMethod]
        [MutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task HoldRequestSuspendTest()
        {
            var response = await Papi.HoldRequestSuspendAsync(Settings.PatronBarcode, 1234, DateTime.Now, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -4201);
        }
    }
}
