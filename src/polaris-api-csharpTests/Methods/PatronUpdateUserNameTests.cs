using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class PatronUpdateUserNameTests : IntegrationTestBase
    {
        [TestMethod]
        [MutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task PatronUpdateUserNameTest()
        {
            var response = await Papi.PatronUpdateUserNameAsync(Settings.PatronBarcode + "1234", Settings.PatronPin, Settings.PatronPin);
            Assert.IsTrue(response.Response.StatusCode == HttpStatusCode.Unauthorized);
        }
    }
}
