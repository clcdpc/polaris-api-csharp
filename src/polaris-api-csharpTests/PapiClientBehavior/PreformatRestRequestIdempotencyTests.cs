using System.Linq;
using System.Net.Http;
using Clc.Polaris.Api;
using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Tests;
using Clc.Polaris.Api.Tests.TestInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.PapiClientBehavior
{
    [TestClass]
    [UnitTest]
    public class PreformatRestRequestIdempotencyTests : PapiClientTestBase
    {
        [TestMethod]
        public void PreformatRestRequest_WhenCalledTwice_ReplacesPapiHeadersWithoutDuplicatingOrChangingBody()
        {
            var client = CreateClient();
            var body = new { Value = "unchanged" };
            var request = new PapiRestRequest(HttpMethod.Put, "/public/v1/1033/100/1/patron/ABC123", "1234", body);
            request.Headers.Add("X-Custom", "keep");
            request.QueryParameters.Add("ignoresa", false);

            var first = (PapiRestRequest)client.PreformatRestRequest(request);
            var firstHeaderCount = first.Headers.Count;
            var second = (PapiRestRequest)client.PreformatRestRequest(first);

            Assert.AreSame(first, second);
            Assert.HasCount(firstHeaderCount, second.Headers);
            Assert.AreEqual("keep", second.Headers["X-Custom"]);
            Assert.AreSame(body, second.Body);
            Assert.ContainsSingle(key => key == "PolarisDate", second.Headers.Keys);
            Assert.ContainsSingle(key => key == "Authorization", second.Headers.Keys);
            var date = second.Headers["PolarisDate"];
            var expectedUri = "https://example.test/PAPIService/REST/public/v1/1033/100/1/patron/ABC123?ignoresa=False";
            var expectedHash = PapiSignature.ComputeHash("access-key", "PUT", expectedUri, date, "1234");
            Assert.AreEqual($"PWS access-id:{expectedHash}", second.Headers["Authorization"]);
        }
    }
}
