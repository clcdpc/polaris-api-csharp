using System;
using System.Net.Http;
using System.Threading.Tasks;
using Clc.Polaris.Api;
using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Tests;
using Clc.Polaris.Api.Tests.TestInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Methods.RequestShape
{
    [TestClass]
    [UnitCategory]
    public class PatronSearchTests : PapiClientTestBase
    {
        [TestMethod]
        public async Task PublicPatronMethod_WithPassword_DoesNotAuthenticateOrSendStaffOverrideHeader()
        {
            var handler = new FailingStaffAuthenticationHttpMessageHandler();
            var client = CreateClient(handler);
            client.AllowStaffOverrideRequests = true;
            client.StaffOverrideAccount = new PolarisUser
            {
                Domain = "main",
                Username = $"password-patron-{Guid.NewGuid():N}",
                Password = "secret"
            };

            var response = await client.PatronMessagesGetAsync("ABC123", password: "patron-password");

            Assert.IsNotNull(response);
            Assert.AreEqual(0, handler.AuthenticationRequestCount);
            Assert.AreEqual(1, handler.PublicRequestCount);
            Assert.IsNotNull(handler.LastNonAuthenticationRequest);
            Assert.IsFalse(handler.LastNonAuthenticationRequest!.Headers.Contains("X-PAPI-AccessToken"));
            AssertAuthorizationHashesSentUri(handler.LastNonAuthenticationRequest, "patron-password");
        }

        [TestMethod]
        public async Task PatronSearch_RequestShape_PreservesEncodedQuerySemantics()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            client.Token = new ProtectedToken { AccessToken = "token", AccessSecret = "secret", ExpirationDate = DateTime.Now.AddHours(1) };

            var response = await client.PatronSearchAsync("name = Smith & status: active", page: 3, pageSize: 25, sortBy: PatronSortKeys.PATNL, orgId: 9);

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest!.Method);
            StringAssert.Contains(handler.LastRequest.RequestUri!.AbsolutePath, "/protected/v1/1033/100/9/token/search/patrons/Boolean");
            var query = ParseQuery(handler.LastRequest.RequestUri.Query);
            Assert.AreEqual("name = Smith & status: active", query["q"]);
            Assert.AreEqual("25", query["patronsperpage"]);
            Assert.AreEqual("3", query["page"]);
            Assert.AreEqual("PATNL", query["sort"]);
            Assert.IsNull(handler.LastRequest.Content);
            Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
        }
    }
}
