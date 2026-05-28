using Clc.Polaris.Api.Configuration;
using Clc.Polaris.Api.Models;
using Clc.Rest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api.Tests;

[TestClass]
[TestCategory("Unit")]
[TestCategory("RestClientMigration")]
public class RestClientMigrationCharacterizationTests
{
    [TestMethod]
    public void PapiRestRequest_ConstructorsAndPathFlags_PreserveCurrentBehavior()
    {
        var getRequest = new PapiRestRequest("/public/foo");
        Assert.AreEqual(HttpMethod.Get, getRequest.Method);
        Assert.AreEqual("/public/foo", getRequest.Path);
        Assert.IsTrue(getRequest.IsPublicMethod);
        Assert.IsFalse(getRequest.IsProtectedMethod);

        var body = new { Name = "abc" };
        var putRequest = new PapiRestRequest(HttpMethod.Put, "/public/foo", "pin", body);
        Assert.AreEqual(HttpMethod.Put, putRequest.Method);
        Assert.AreEqual("/public/foo", putRequest.Path);
        Assert.AreEqual("pin", putRequest.Password);
        Assert.AreSame(body, putRequest.Body);

        var restRequest = new RestRequest
        {
            Method = HttpMethod.Post,
            Path = "/protected/foo",
            Body = body,
            Parameters = new Dictionary<string, string> { ["q"] = "dogs" },
            Headers = new Dictionary<string, string> { ["X-Test"] = "1" }
        };

        var converted = new PapiRestRequest(restRequest);
        Assert.AreEqual(HttpMethod.Post, converted.Method);
        Assert.AreEqual("/protected/foo", converted.Path);
        Assert.AreSame(body, converted.Body);
        Assert.AreSame(restRequest.Parameters, converted.Parameters);
        Assert.AreSame(restRequest.Headers, converted.Headers);
        Assert.IsFalse(converted.IsPublicMethod);
        Assert.IsTrue(converted.IsProtectedMethod);
    }

    [TestMethod]
    public void PreformatRestRequest_AddsRequiredHeaders_UnlessAuthDisabled()
    {
        var client = BuildClient();
        var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/apikeyvalidate");

        var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

        Assert.AreEqual(HttpMethod.Get, formatted.Method);
        Assert.AreEqual("/public/v1/1033/100/1/apikeyvalidate", formatted.Path);
        Assert.IsTrue(formatted.Headers.ContainsKey("PolarisDate"));
        Assert.IsTrue(formatted.Headers.ContainsKey("Authorization"));
        StringAssert.StartsWith(formatted.Headers["Authorization"], "PWS access-id:");

        var authDisabled = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/apikeyvalidate") { AuthRequired = false };
        var noAuth = (PapiRestRequest)client.PreformatRestRequest(authDisabled);
        Assert.IsFalse(noAuth.Headers.ContainsKey("PolarisDate"));
        Assert.IsFalse(noAuth.Headers.ContainsKey("Authorization"));
    }

    [TestMethod]
    public void PreformatRestRequest_StaffOverrideTokenBehavior_MatchesCurrentRules()
    {
        var client = BuildClient();
        client.AllowStaffOverrideRequests = true;
        client.Token = new ProtectedToken
        {
            AccessToken = "override-token",
            AccessSecret = "override-secret",
            ExpirationDate = DateTime.UtcNow.AddHours(1)
        };

        var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/apikeyvalidate");
        var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

        Assert.IsTrue(formatted.Headers.ContainsKey("X-PAPI-AccessToken"));
        Assert.AreEqual("override-token", formatted.Headers["X-PAPI-AccessToken"]);
        Assert.IsTrue(formatted.Headers.ContainsKey("Authorization"));

        var blocked = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/apikeyvalidate") { BlockStaffOverride = true };
        var blockedFormatted = (PapiRestRequest)client.PreformatRestRequest(blocked);
        Assert.IsFalse(blockedFormatted.Headers.ContainsKey("X-PAPI-AccessToken"));
        Assert.IsTrue(blockedFormatted.Headers.ContainsKey("Authorization"));
    }

    [TestMethod]
    public void RequestShape_ApiKeyValidate_IsGetPublicNoBody()
    {
        var handler = new CapturingHttpMessageHandler(_ => Json("{\"PAPIErrorCode\":0}"));
        var client = BuildClient(handler);

        var response = client.ApiKeyValidate();
        Assert.IsNotNull(response);

        var request = handler.LastRequest!;
        Assert.AreEqual(HttpMethod.Get, request.Method);
        StringAssert.Contains(request.RequestUri!.AbsoluteUri, "/public/v1/1033/100/1/apikeyvalidate");
        Assert.IsNull(request.Content);
        Assert.IsTrue(request.Headers.Contains("Authorization"));
        Assert.IsTrue(request.Headers.Contains("PolarisDate"));
    }

    [TestMethod]
    public void RequestShape_BibSearch_IsGetWithExpectedEncodedQuery()
    {
        var handler = new CapturingHttpMessageHandler(_ => Json("{\"PAPIErrorCode\":0}"));
        var client = BuildClient(handler);

        client.BibSearch(new BibSearchOptions
        {
            Branch = 7,
            Term = "dogs & cats",
            Page = 2,
            PageSize = 5,
            SortOption = SearchSortOptions.MP,
            Limit = "FIC"
        });

        var request = handler.LastRequest!;
        Assert.AreEqual(HttpMethod.Get, request.Method);
        StringAssert.Contains(request.RequestUri!.AbsoluteUri, "/public/v1/1033/100/7/search/bibs/keyword/KW");
        var query = request.RequestUri.Query;
        StringAssert.Contains(query, "q=dogs+%26+cats");
        StringAssert.Contains(query, "sort=MP");
        StringAssert.Contains(query, "page=2");
        StringAssert.Contains(query, "bibsperpage=5");
        StringAssert.Contains(query, "limit=FIC");
        Assert.IsNull(request.Content);
        Assert.IsTrue(request.Headers.Contains("Authorization"));
        Assert.IsTrue(request.Headers.Contains("PolarisDate"));
    }

    [TestMethod]
    public void RequestShape_AuthenticateStaffUser_IsPostWithBodyToProtectedEndpoint()
    {
        var handler = new CapturingHttpMessageHandler(_ => Json("{\"PAPIErrorCode\":0,\"AccessToken\":\"tok\",\"AccessSecret\":\"sec\"}"));
        var client = BuildClient(handler);

        client.AuthenticateStaffUser(new PolarisUser { Domain = "d", Username = "u", Password = "p" });

        var request = handler.LastRequest!;
        Assert.AreEqual(HttpMethod.Post, request.Method);
        StringAssert.Contains(request.RequestUri!.AbsoluteUri, "/protected/v1/1033/100/1/authenticator/staff");
        Assert.IsNotNull(request.Content);
        var body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
        StringAssert.Contains(body, "\"Domain\":\"d\"");
        StringAssert.Contains(body, "\"Username\":\"u\"");
        Assert.IsTrue(request.Headers.Contains("Authorization"));
        Assert.IsTrue(request.Headers.Contains("PolarisDate"));
    }

    [TestMethod]
    public void RequestShape_PatronUpdate_IsPutWithBarcodeQueryAndBody()
    {
        var handler = new CapturingHttpMessageHandler(_ => Json("{\"PAPIErrorCode\":0}"));
        var client = BuildClient(handler);

        client.PatronUpdate("123456", new PatronUpdateParams { EmailAddress = "patron@example.test" }, password: "1234", ignoresa: true);

        var request = handler.LastRequest!;
        Assert.AreEqual(HttpMethod.Put, request.Method);
        StringAssert.Contains(request.RequestUri!.AbsoluteUri, "/public/v1/1033/100/1/patron/123456");
        StringAssert.Contains(request.RequestUri.Query, "ignoresa=True");
        Assert.IsNotNull(request.Content);
        var body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
        StringAssert.Contains(body, "\"EmailAddress\":\"patron@example.test\"");
        Assert.IsTrue(request.Headers.Contains("Authorization"));
        Assert.IsTrue(request.Headers.Contains("PolarisDate"));
    }

    private static PapiClient BuildClient(HttpMessageHandler? handler = null)
    {
        var httpClient = handler is null ? new HttpClient() : new HttpClient(handler);
        return new PapiClient(httpClient, new TestPapiSettings());
    }

    private static HttpResponseMessage Json(string payload) =>
        new(HttpStatusCode.OK)
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };

    private sealed class TestPapiSettings : IPapiSettings
    {
        public string AccessId { get; set; } = "access-id";
        public string AccessKey { get; set; } = "access-key";
        public string Hostname { get; set; } = "https://example.test";
        public int OrganizationId { get; set; } = 1;
        public int UserId { get; set; } = 1;
        public int WorkstationId { get; set; } = 1;
        public PolarisUser PolarisOverrideAccount { get; set; } = null!;
    }

    private sealed class CapturingHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responseFactory;

        public CapturingHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _responseFactory = responseFactory;
        }

        public HttpRequestMessage? LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(_responseFactory(request));
        }
    }
}
