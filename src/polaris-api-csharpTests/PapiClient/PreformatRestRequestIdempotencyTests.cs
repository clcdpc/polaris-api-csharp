using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Clc.Polaris.Api;
using Clc.Polaris.Api.Configuration;
using Clc.Polaris.Api.Models;
using Clc.Rest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    [UnitCategory]
    public class PreformatRestRequestIdempotencyTests : RestClientMigrationTestBase
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
                    Assert.AreEqual(firstHeaderCount, second.Headers.Count);
                    Assert.AreEqual("keep", second.Headers["X-Custom"]);
                    Assert.AreSame(body, second.Body);
                    Assert.AreEqual(1, second.Headers.Keys.Count(key => key == "PolarisDate"));
                    Assert.AreEqual(1, second.Headers.Keys.Count(key => key == "Authorization"));
                    var date = second.Headers["PolarisDate"];
                    var expectedUri = "https://example.test/PAPIService/REST/public/v1/1033/100/1/patron/ABC123?ignoresa=False";
                    var expectedHash = PapiSignature.ComputeHash("access-key", "PUT", expectedUri, date, "1234");
                    Assert.AreEqual($"PWS access-id:{expectedHash}", second.Headers["Authorization"]);
                }

                [TestMethod]
                public void PreformatRestRequest_AuthorizationChanges_WhenQueryParameterValueChanges()
                {
                    var client = CreateClient();
                    var first = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/search/bibs/keyword/KW");
                    first.QueryParameters.Add("q", "harry potter");
                    var second = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/search/bibs/keyword/KW");
                    second.QueryParameters.Add("q", "lord of the rings");
        
                    var firstFormatted = (PapiRestRequest)client.PreformatRestRequest(first);
                    var secondFormatted = (PapiRestRequest)client.PreformatRestRequest(second);
        
                    var firstDate = firstFormatted.Headers["PolarisDate"];
                    var secondDate = secondFormatted.Headers["PolarisDate"];
                    var firstExpectedHash = PapiSignature.ComputeHash("access-key", "GET", "https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?q=harry%20potter", firstDate, string.Empty);
                    var secondExpectedHash = PapiSignature.ComputeHash("access-key", "GET", "https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?q=lord%20of%20the%20rings", secondDate, string.Empty);
                    Assert.AreEqual($"PWS access-id:{firstExpectedHash}", firstFormatted.Headers["Authorization"]);
                    Assert.AreEqual($"PWS access-id:{secondExpectedHash}", secondFormatted.Headers["Authorization"]);
                    Assert.AreNotEqual(firstFormatted.Headers["Authorization"], secondFormatted.Headers["Authorization"]);
                }
    }
}
