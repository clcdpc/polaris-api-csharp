using System.Net;

namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public sealed class PatronUpdateUserNameTests : IntegrationTestBase
    {
        [TestMethod]
        [MutatingLiveTest]
        [DoNotParallelize]
        public async Task PatronUpdateUserNameTest()
        {
            var response = await Papi.PatronUpdateUserNameAsync(Settings.PatronBarcode + "1234", Settings.PatronPin, Settings.PatronPin, TestContext.CancellationToken);
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.Response.StatusCode);
        }
    }
}
