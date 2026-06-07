using System.Threading.Tasks;
using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronMessageUpdateStatusTests : IntegrationTestBase
    {
        [TestMethod]
        [MutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task PatronMessageUpdateStatusTest()
        {
            var response = await Papi.PatronMessageUpdateStatusAsync(Settings.PatronBarcode, PatronMessageType.freetext, 1234, Settings.PatronPin);
            Assert.IsTrue(response.Data.PAPIErrorCode == -1);
        }
    }
}
