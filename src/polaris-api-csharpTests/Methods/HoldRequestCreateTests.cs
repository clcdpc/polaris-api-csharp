using Clc.Polaris.Api;
using Clc.Polaris.Api.Models;
using PapiClientType = Clc.Polaris.Api.PapiClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class HoldRequestCreateTests : IntegrationTestBase
    {
        [TestMethod]
        [MutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task HoldRequestCreateTest()
        {
            var response = await Papi.HoldRequestCreateAsync(new HoldRequestCreateParams(Settings.PatronId, 1234, 7, 7));
            Assert.IsTrue(response.Data.PAPIErrorCode == -4006);
        }

        [TestMethod]
        [MutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task HoldRequestCreateTest2()
        {
            var response = await ((PapiClientType)Papi).HoldRequestCreateAsync(Settings.PatronId, 1234, 7);
            Assert.IsTrue(response.Data.PAPIErrorCode == -4006);
        }
    }
}
