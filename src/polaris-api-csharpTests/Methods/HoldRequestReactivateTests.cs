using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class HoldRequestReactivateTests : IntegrationTestBase
    {
        [TestMethod()]
        [MutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task HoldRequestReactivateTest()
        {
            var response = await Papi.HoldRequestReactivateAsync(Settings.PatronBarcode, Settings.PatronPin, 1234, DateTime.Now);
            Assert.IsTrue(response.Data.PAPIErrorCode == -4201);
        }
    }
}
