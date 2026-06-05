using Clc.Polaris.Api.Configuration;
using Clc.Polaris.Api.Models;
using Clc.Rest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    [TestCategory("Unit")]
    [TestCategory("RestClientMigration")]
    public class RestClientMigrationCharacterizationTests
    {
        [TestMethod]
        public void PapiRestRequest_Constructors_PreserveCurrentBehavior()
        {
            var defaultGet = new PapiRestRequest("/public/foo");
            Assert.AreEqual(HttpMethod.Get, defaultGet.Method);
            Assert.AreEqual("/public/foo", defaultGet.Path);

            var body = new { Name = "Example" };
            var put = new PapiRestRequest(HttpMethod.Put, "/public/foo", "pin", body);
            Assert.AreEqual(HttpMethod.Put, put.Method);
            Assert.AreEqual("/public/foo", put.Path);
            Assert.AreEqual("pin", put.Password);
            Assert.AreSame(body, put.Body);

            var existing = new RestRequest
            {
                Method = HttpMethod.Post,
                Path = "/protected/foo",
                Body = new { Value = "v" },
            };
            existing.QueryParameters.Add("limit", "5");
            existing.Headers.Add("X-Test", "header");

            var copied = new PapiRestRequest(existing);
            Assert.AreEqual(existing.Method, copied.Method);
            Assert.AreEqual(existing.Path, copied.Path);
            Assert.AreSame(existing.Body, copied.Body);
            Assert.AreSame(existing.QueryParameters, copied.QueryParameters);
            Assert.AreSame(existing.Headers, copied.Headers);
        }

        [TestMethod]
        public void PapiRestRequest_PathClassification_IsStable()
        {
            var publicRequest = new PapiRestRequest("/public/foo");
            Assert.IsTrue(publicRequest.IsPublicMethod);
            Assert.IsFalse(publicRequest.IsProtectedMethod);

            var protectedRequest = new PapiRestRequest("/protected/foo");
            Assert.IsFalse(protectedRequest.IsPublicMethod);
            Assert.IsTrue(protectedRequest.IsProtectedMethod);
        }

        [TestMethod]
        public void PreformatRestRequest_AddsPapiHeaders_WhenAuthRequired()
        {
            var client = CreateClient();
            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/apikeyvalidate");

            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            Assert.AreEqual(HttpMethod.Get, formatted.Method);
            Assert.AreEqual("/public/v1/1033/100/1/apikeyvalidate", formatted.Path);
            Assert.IsTrue(formatted.Headers.ContainsKey("PolarisDate"));
            Assert.IsTrue(formatted.Headers.ContainsKey("Authorization"));
            StringAssert.StartsWith(formatted.Headers["Authorization"], "PWS access-id:");
        }

        [TestMethod]
        public void PreformatRestRequest_DoesNotAddPapiHeaders_WhenAuthNotRequired()
        {
            var client = CreateClient();
            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/apikeyvalidate")
            {
                AuthRequired = false
            };

            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            Assert.IsFalse(formatted.Headers.ContainsKey("PolarisDate"));
            Assert.IsFalse(formatted.Headers.ContainsKey("Authorization"));
        }

        [TestMethod]
        public void PreformatRestRequest_PublicAuthenticatedGetWithoutQueryParameters_HashesOriginalUrl()
        {
            var client = CreateClient();
            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/apikeyvalidate");

            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            var date = formatted.Headers["PolarisDate"];
            var expectedUri = "https://example.test/PAPIService/REST/public/v1/1033/100/1/apikeyvalidate";
            var expectedHash = ComputePapiHash("GET", expectedUri, date, string.Empty, "access-key");
            Assert.AreEqual($"PWS access-id:{expectedHash}", formatted.Headers["Authorization"]);
            Assert.AreSame(request.Body, formatted.Body);
        }

        [TestMethod]
        public void PreformatRestRequest_ProtectedGet_UsesProtectedTokenSecretForHash()
        {
            var client = CreateClient();
            client.Token = new ProtectedToken
            {
                AccessToken = "protected-token",
                AccessSecret = "protected-secret",
                ExpirationDate = DateTime.Now.AddHours(1)
            };
            var request = new PapiRestRequest(HttpMethod.Get, "/protected/v1/1033/100/1/protected-token/search/patrons/Boolean");

            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            var date = formatted.Headers["PolarisDate"];
            var expectedUri = "https://example.test/PAPIService/REST/protected/v1/1033/100/1/protected-token/search/patrons/Boolean";
            var expectedHash = ComputePapiHash("GET", expectedUri, date, "protected-secret", "access-key");
            Assert.AreEqual($"PWS access-id:{expectedHash}", formatted.Headers["Authorization"]);
            Assert.IsFalse(formatted.Headers.ContainsKey("X-PAPI-AccessToken"));
        }

        [TestMethod]
        public void PreformatRestRequest_PublicAuthenticatedGetWithQueryParameters_HashesEffectiveOutgoingUrl()
        {
            var client = CreateClient();
            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/search/bibs/keyword/KW");
            request.QueryParameters.Add("q", "harry potter & stone");
            request.QueryParameters.Add("limit", "branch:1");

            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            var date = formatted.Headers["PolarisDate"];
            var expectedUri = "https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?q=harry%20potter%20%26%20stone&limit=branch%3A1";
            var expectedHash = ComputePapiHash("GET", expectedUri, date, string.Empty, "access-key");
            Assert.AreEqual($"PWS access-id:{expectedHash}", formatted.Headers["Authorization"]);
        }

        [TestMethod]
        public void PreformatRestRequest_AuthenticatedPutWithBodyAndQueryParameters_HashesQueryAndPreservesBody()
        {
            var client = CreateClient();
            var body = new PatronUpdateParams { EmailAddress = "patron@example.test" };
            var request = new PapiRestRequest(HttpMethod.Put, "/public/v1/1033/100/1/patron/ABC123", "1234", body);
            request.QueryParameters.Add("ignoresa", true);

            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            var date = formatted.Headers["PolarisDate"];
            var expectedUri = "https://example.test/PAPIService/REST/public/v1/1033/100/1/patron/ABC123?ignoresa=True";
            var expectedHash = ComputePapiHash("PUT", expectedUri, date, "1234", "access-key");
            Assert.AreEqual($"PWS access-id:{expectedHash}", formatted.Headers["Authorization"]);
            Assert.AreSame(body, formatted.Body);
        }

        [TestMethod]
        public void PreformatRestRequest_AuthenticatedPostWithBodyAndQueryParameters_HashesQueryAndPreservesBody()
        {
            var client = CreateClient();
            var body = new { Amount = 12.34m };
            var request = new PapiRestRequest(HttpMethod.Post, "/protected/v1/1033/100/1/token/patron/123/account/payment", "protected-secret", body);
            request.QueryParameters.Add("wsid", 7);
            request.QueryParameters.Add("userid", 8);

            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            var date = formatted.Headers["PolarisDate"];
            var expectedUri = "https://example.test/PAPIService/REST/protected/v1/1033/100/1/token/patron/123/account/payment?wsid=7&userid=8";
            var expectedHash = ComputePapiHash("POST", expectedUri, date, "protected-secret", "access-key");
            Assert.AreEqual($"PWS access-id:{expectedHash}", formatted.Headers["Authorization"]);
            Assert.AreSame(body, formatted.Body);
        }

        [TestMethod]
        public void PreformatRestRequest_AddsStaffOverrideToken_WhenAllowedAndUnblocked()
        {
            var client = CreateClient();
            client.AllowStaffOverrideRequests = true;
            client.Token = new ProtectedToken
            {
                AccessToken = "staff-token",
                AccessSecret = "staff-secret",
                ExpirationDate = DateTime.Now.AddHours(1)
            };

            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/apikeyvalidate");
            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            Assert.IsTrue(formatted.Headers.ContainsKey("X-PAPI-AccessToken"));
            Assert.AreEqual("staff-token", formatted.Headers["X-PAPI-AccessToken"]);
            var date = formatted.Headers["PolarisDate"];
            var expectedUri = "https://example.test/PAPIService/REST/public/v1/1033/100/1/apikeyvalidate";
            var expectedHash = ComputePapiHash("GET", expectedUri, date, "staff-secret", "access-key");
            Assert.AreEqual($"PWS access-id:{expectedHash}", formatted.Headers["Authorization"]);
        }

        [TestMethod]
        public void PreformatRestRequest_DoesNotAddStaffOverrideToken_WhenBlocked()
        {
            var client = CreateClient();
            client.AllowStaffOverrideRequests = true;
            client.Token = new ProtectedToken
            {
                AccessToken = "staff-token",
                AccessSecret = "staff-secret",
                ExpirationDate = DateTime.Now.AddHours(1)
            };

            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/apikeyvalidate")
            {
                BlockStaffOverride = true
            };
            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            Assert.IsFalse(formatted.Headers.ContainsKey("X-PAPI-AccessToken"));
            var date = formatted.Headers["PolarisDate"];
            var expectedUri = "https://example.test/PAPIService/REST/public/v1/1033/100/1/apikeyvalidate";
            var expectedHash = ComputePapiHash("GET", expectedUri, date, string.Empty, "access-key");
            Assert.AreEqual($"PWS access-id:{expectedHash}", formatted.Headers["Authorization"]);
        }

        [TestMethod]
        public void PreformatRestRequest_DoesNotAddStaffOverrideToken_WhenDisabled()
        {
            var client = CreateClient();
            client.AllowStaffOverrideRequests = false;
            client.Token = new ProtectedToken
            {
                AccessToken = "staff-token",
                AccessSecret = "staff-secret",
                ExpirationDate = DateTime.Now.AddHours(1)
            };

            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/apikeyvalidate");
            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            Assert.IsFalse(formatted.Headers.ContainsKey("X-PAPI-AccessToken"));
            var date = formatted.Headers["PolarisDate"];
            var expectedUri = "https://example.test/PAPIService/REST/public/v1/1033/100/1/apikeyvalidate";
            var expectedHash = ComputePapiHash("GET", expectedUri, date, string.Empty, "access-key");
            Assert.AreEqual($"PWS access-id:{expectedHash}", formatted.Headers["Authorization"]);
        }

        [TestMethod]
        public void PreformatRestRequest_RemovesStaleStaffOverrideToken_WhenOverrideBecomesBlocked()
        {
            var client = CreateClient();
            client.AllowStaffOverrideRequests = true;
            client.Token = new ProtectedToken
            {
                AccessToken = "staff-token",
                AccessSecret = "staff-secret",
                ExpirationDate = DateTime.Now.AddHours(1)
            };
            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/apikeyvalidate");

            var first = (PapiRestRequest)client.PreformatRestRequest(request);
            Assert.AreEqual("staff-token", first.Headers["X-PAPI-AccessToken"]);

            request.BlockStaffOverride = true;
            var second = (PapiRestRequest)client.PreformatRestRequest(request);

            Assert.AreSame(first, second);
            Assert.IsFalse(second.Headers.ContainsKey("X-PAPI-AccessToken"));
            var date = second.Headers["PolarisDate"];
            var expectedUri = "https://example.test/PAPIService/REST/public/v1/1033/100/1/apikeyvalidate";
            var expectedHash = ComputePapiHash("GET", expectedUri, date, string.Empty, "access-key");
            Assert.AreEqual($"PWS access-id:{expectedHash}", second.Headers["Authorization"]);
        }

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
            var expectedHash = ComputePapiHash("PUT", expectedUri, date, "1234", "access-key");
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
            var firstExpectedHash = ComputePapiHash("GET", "https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?q=harry%20potter", firstDate, string.Empty, "access-key");
            var secondExpectedHash = ComputePapiHash("GET", "https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?q=lord%20of%20the%20rings", secondDate, string.Empty, "access-key");
            Assert.AreEqual($"PWS access-id:{firstExpectedHash}", firstFormatted.Headers["Authorization"]);
            Assert.AreEqual($"PWS access-id:{secondExpectedHash}", secondFormatted.Headers["Authorization"]);
            Assert.AreNotEqual(firstFormatted.Headers["Authorization"], secondFormatted.Headers["Authorization"]);
        }

        [TestMethod]
        public async Task ApiKeyValidate_RequestShape_IsStable()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);

            var response = await client.ApiKeyValidateAsync();

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest!.Method);
            StringAssert.Contains(handler.LastRequest.RequestUri!.AbsolutePath, "/public/v1/1033/100/1/apikeyvalidate");
            Assert.AreEqual(string.Empty, handler.LastRequest.RequestUri.Query);
            Assert.IsNull(handler.LastRequest.Content);
            Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
        }


        [TestMethod]
        public async Task ApiKeyValidateAsync_PassesCancellationToken()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            using var cts = new CancellationTokenSource();

            var response = await client.ApiKeyValidateAsync(cts.Token);

            Assert.IsNotNull(response);
            Assert.IsTrue(handler.LastCancellationToken.CanBeCanceled);
        }


        [TestMethod]
        public async Task PatronSearchAsync_PassesCancellationTokenToProtectedTokenAcquisition()
        {
            var handler = new ProtectedTokenCancellationHttpMessageHandler();
            var client = CreateClient(handler);
            client.AllowStaffOverrideRequests = true;
            client.StaffOverrideAccount = new PolarisUser
            {
                Domain = "main",
                Username = "staff",
                Password = "secret"
            };
            using var cts = new CancellationTokenSource();

            var response = await client.PatronSearchAsync("name=Smith", cancellationToken: cts.Token);

            Assert.IsNotNull(response);
            Assert.AreEqual(2, handler.Requests.Count);
            StringAssert.Contains(handler.Requests[0].RequestUri!.AbsolutePath, "/protected/v1/1033/100/1/authenticator/staff");
            Assert.AreEqual(HttpMethod.Post, handler.Requests[0].Method);
            Assert.IsTrue(handler.CancellationTokens[0].CanBeCanceled);
            StringAssert.Contains(handler.Requests[1].RequestUri!.AbsolutePath, "/protected/v1/1033/100/1/protected-token/search/patrons/Boolean");
            Assert.IsTrue(handler.CancellationTokens[1].CanBeCanceled);
        }


        [TestMethod]
        public async Task ProtectedTokenPathRequest_ReplacesPlaceholderBeforeSending()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            client.Token = new ProtectedToken { AccessToken = "real-token", AccessSecret = "real-secret", ExpirationDate = DateTime.Now.AddHours(1) };

            var response = await client.PatronSearchAsync("name=Smith", orgId: 9);

            Assert.IsNotNull(response);
            Assert.IsNotNull(handler.LastRequest);
            Assert.IsFalse(handler.LastRequest!.RequestUri!.AbsoluteUri.Contains(ProtectedToken.Placeholder, StringComparison.Ordinal));
            StringAssert.Contains(handler.LastRequest.RequestUri.AbsolutePath, "/protected/v1/1033/100/9/real-token/search/patrons/Boolean");
        }

        [TestMethod]
        public async Task ProtectedTokenPathRequest_HashesReplacedUrlInsteadOfPlaceholderUrl()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            client.Token = new ProtectedToken { AccessToken = "hash-token", AccessSecret = "hash-secret", ExpirationDate = DateTime.Now.AddHours(1) };

            await client.PatronSearchAsync("name=Smith", orgId: 9);

            Assert.IsNotNull(handler.LastRequest);
            var date = handler.LastRequest!.Headers.GetValues("PolarisDate").Single();
            var expectedUri = "https://example.test/PAPIService/REST/protected/v1/1033/100/9/hash-token/search/patrons/Boolean?q=name%3DSmith&patronsperpage=10&page=1&sort=PATN";
            var expectedHash = ComputePapiHash("GET", expectedUri, date, "hash-secret", "access-key");
            var placeholderUri = expectedUri.Replace("hash-token", ProtectedToken.Placeholder, StringComparison.Ordinal);
            var placeholderHash = ComputePapiHash("GET", placeholderUri, date, "hash-secret", "access-key");
            Assert.AreEqual($"PWS access-id:{expectedHash}", handler.LastRequest.Headers.GetValues("Authorization").Single());
            Assert.AreNotEqual($"PWS access-id:{placeholderHash}", handler.LastRequest.Headers.GetValues("Authorization").Single());
        }

        [TestMethod]
        public async Task ProtectedTokenPathRequest_AcquiresProtectedTokenAutomaticallyWhenMissing()
        {
            var handler = new ProtectedTokenCancellationHttpMessageHandler();
            var client = CreateClient(handler);
            client.AllowStaffOverrideRequests = true;
            client.StaffOverrideAccount = new PolarisUser
            {
                Domain = "main",
                Username = "auto-placeholder",
                Password = "secret"
            };

            var response = await client.PatronSearchAsync("name=Smith", orgId: 9);

            Assert.IsNotNull(response);
            Assert.AreEqual(2, handler.Requests.Count);
            StringAssert.Contains(handler.Requests[0].RequestUri!.AbsolutePath, "/protected/v1/1033/100/1/authenticator/staff");
            StringAssert.Contains(handler.Requests[1].RequestUri!.AbsolutePath, "/protected/v1/1033/100/9/protected-token/search/patrons/Boolean");
            Assert.IsFalse(handler.Requests[1].RequestUri!.AbsoluteUri.Contains(ProtectedToken.Placeholder, StringComparison.Ordinal));
        }


        [TestMethod]
        public async Task ProtectedTokenPathRequest_WithExpiredToken_AcquiresProtectedTokenBeforePlaceholderReplacement()
        {
            var handler = new ProtectedTokenCancellationHttpMessageHandler();
            var client = CreateClient(handler);
            client.AllowStaffOverrideRequests = true;
            client.StaffOverrideAccount = new PolarisUser
            {
                Domain = "main",
                Username = $"expired-placeholder-{Guid.NewGuid():N}",
                Password = "secret"
            };
            client.Token = new ProtectedToken
            {
                AccessToken = "expired-token",
                AccessSecret = "expired-secret",
                ExpirationDate = DateTime.Now.AddHours(-1)
            };

            var response = await client.PatronSearchAsync("name=Smith", orgId: 9);

            Assert.IsNotNull(response);
            Assert.AreEqual(2, handler.Requests.Count);
            StringAssert.Contains(handler.Requests[0].RequestUri!.AbsolutePath, "/protected/v1/1033/100/1/authenticator/staff");
            StringAssert.Contains(handler.Requests[1].RequestUri!.AbsolutePath, "/protected/v1/1033/100/9/protected-token/search/patrons/Boolean");
            Assert.IsFalse(handler.Requests[1].RequestUri!.AbsoluteUri.Contains(ProtectedToken.Placeholder, StringComparison.Ordinal));
            Assert.AreEqual("protected-token", client.Token!.AccessToken);
        }

        [TestMethod]
        public async Task ProtectedTokenPathRequest_WhenAuthenticationCannotProvideValidToken_ThrowsPlaceholderExceptionBeforeProtectedRequest()
        {
            var handler = new FailingStaffAuthenticationHttpMessageHandler();
            var client = CreateClient(handler);
            client.AllowStaffOverrideRequests = true;
            client.StaffOverrideAccount = new PolarisUser
            {
                Domain = "main",
                Username = $"failed-placeholder-{Guid.NewGuid():N}",
                Password = "secret"
            };
            client.Token = new ProtectedToken
            {
                AccessToken = "expired-token",
                AccessSecret = "expired-secret",
                ExpirationDate = DateTime.Now.AddHours(-1)
            };

            var exception = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => client.PatronSearchAsync("name=Smith", orgId: 9));

            Assert.AreEqual("A valid protected access token is required to replace ProtectedToken.Placeholder in the request path.", exception.Message);
            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(0, handler.ProtectedRequestCount);
            Assert.IsNull(client.Token);
        }

        [TestMethod]
        public async Task ProtectedTokenPathRequest_UsesCachedProtectedTokenForPlaceholderReplacement()
        {
            var handler = new ProtectedTokenCancellationHttpMessageHandler();
            var firstClient = CreateClient(handler);
            firstClient.AllowStaffOverrideRequests = true;
            firstClient.UseProtectedTokenCache = true;
            firstClient.StaffOverrideAccount = new PolarisUser
            {
                Domain = "main",
                Username = $"cached-placeholder-{Guid.NewGuid():N}",
                Password = "secret"
            };

            await firstClient.PatronSearchAsync("name=Smith", orgId: 9);

            var secondClient = CreateClient(handler);
            secondClient.AllowStaffOverrideRequests = true;
            secondClient.UseProtectedTokenCache = true;
            secondClient.StaffOverrideAccount = new PolarisUser
            {
                Domain = firstClient.StaffOverrideAccount.Domain,
                Username = firstClient.StaffOverrideAccount.Username,
                Password = "secret"
            };

            var response = await secondClient.PatronSearchAsync("name=Jones", orgId: 9);

            Assert.IsNotNull(response);
            Assert.AreEqual(3, handler.Requests.Count);
            StringAssert.Contains(handler.Requests[0].RequestUri!.AbsolutePath, "/protected/v1/1033/100/1/authenticator/staff");
            StringAssert.Contains(handler.Requests[1].RequestUri!.AbsolutePath, "/protected/v1/1033/100/9/protected-token/search/patrons/Boolean");
            StringAssert.Contains(handler.Requests[2].RequestUri!.AbsolutePath, "/protected/v1/1033/100/9/protected-token/search/patrons/Boolean");
            Assert.IsFalse(handler.Requests[2].RequestUri!.AbsoluteUri.Contains(ProtectedToken.Placeholder, StringComparison.Ordinal));
        }

        [TestMethod]
        public async Task ProtectedTokenPathRequest_DoesNotShareCachedTokenAcrossStaffUsers()
        {
            var handler = new SequencedProtectedTokenHttpMessageHandler();
            var cacheSuffix = Guid.NewGuid().ToString("N");
            var firstClient = CreateClient(handler);
            firstClient.AllowStaffOverrideRequests = true;
            firstClient.UseProtectedTokenCache = true;
            firstClient.StaffOverrideAccount = new PolarisUser
            {
                Domain = "main",
                Username = $"cache-isolation-a-{cacheSuffix}",
                Password = "secret"
            };
            var secondClient = CreateClient(handler);
            secondClient.AllowStaffOverrideRequests = true;
            secondClient.UseProtectedTokenCache = true;
            secondClient.StaffOverrideAccount = new PolarisUser
            {
                Domain = "main",
                Username = $"cache-isolation-b-{cacheSuffix}",
                Password = "secret"
            };

            await firstClient.PatronSearchAsync("name=Smith", orgId: 9);
            await secondClient.PatronSearchAsync("name=Jones", orgId: 9);

            Assert.AreEqual(2, handler.AuthenticationRequestCount);
            Assert.AreEqual(2, handler.ProtectedRequests.Count);
            StringAssert.Contains(handler.ProtectedRequests[0].RequestUri!.AbsolutePath, "/protected/v1/1033/100/9/protected-token-1/search/patrons/Boolean");
            StringAssert.Contains(handler.ProtectedRequests[1].RequestUri!.AbsolutePath, "/protected/v1/1033/100/9/protected-token-2/search/patrons/Boolean");
        }

        [TestMethod]
        public async Task ProtectedTokenPathRequest_WithoutTokenOrStaffOverride_FailsBeforeSendingProtectedRequest()
        {
            var handler = new FailingStaffAuthenticationHttpMessageHandler();
            var client = CreateClient(handler);
            client.AllowStaffOverrideRequests = true;
            client.StaffOverrideAccount = null;
            client.Token = null;

            var exception = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => client.PatronSearchAsync("name=Smith", orgId: 9));

            Assert.AreEqual("A valid protected access token is required to replace ProtectedToken.Placeholder in the request path.", exception.Message);
            Assert.AreEqual(0, handler.AuthenticationRequestCount);
            Assert.AreEqual(0, handler.ProtectedRequestCount);
        }

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
        public async Task ProtectedTokenAcquisition_CanceledWaiterDoesNotHoldCacheLock()
        {
            var handler = new BlockingProtectedTokenHttpMessageHandler();
            var cacheUsername = $"cancel-waiter-{Guid.NewGuid():N}";
            var firstClient = CreateClientWithProtectedCache(handler, cacheUsername);
            var secondClient = CreateClientWithProtectedCache(handler, cacheUsername);
            var thirdClient = CreateClientWithProtectedCache(handler, cacheUsername);

            var firstRequest = firstClient.PatronSearchAsync("name=First", orgId: 9);
            await handler.AuthenticationStarted.Task.ConfigureAwait(false);
            using var secondCts = new CancellationTokenSource();
            var secondRequest = secondClient.PatronSearchAsync("name=Second", orgId: 9, cancellationToken: secondCts.Token);

            secondCts.Cancel();
            try
            {
                await secondRequest.ConfigureAwait(false);
                Assert.Fail("The second protected request should observe cancellation while waiting for the protected-token cache lock.");
            }
            catch (OperationCanceledException)
            {
            }

            handler.CompleteAuthentication.SetResult();
            await firstRequest.ConfigureAwait(false);

            var laterResponse = await thirdClient.PatronSearchAsync("name=Later", orgId: 9);

            Assert.IsNotNull(laterResponse);
            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(2, handler.ProtectedRequests.Count);
            StringAssert.Contains(handler.ProtectedRequests[1].RequestUri!.AbsolutePath, "/protected/v1/1033/100/9/blocked-token/search/patrons/Boolean");
            Assert.IsFalse(handler.ProtectedRequests[1].RequestUri!.AbsoluteUri.Contains(ProtectedToken.Placeholder, StringComparison.Ordinal));
        }

        [TestMethod]
        public async Task PatronReadingHistoryClearAsync_EnumeratesIdsOnceAndSendsCommaSeparatedIds()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            var ids = new ThrowOnSecondEnumerationEnumerable(new[] { 101, 202, 303 });

            var response = await client.PatronReadingHistoryClearAsync("ABC123", "patron-password", ids);

            Assert.IsNotNull(response);
            Assert.AreEqual(1, ids.EnumerationCount);
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Delete, handler.LastRequest!.Method);
            var query = ParseQuery(handler.LastRequest.RequestUri!.Query);
            Assert.AreEqual("101,202,303", query["ids"]);
            AssertAuthorizationHashesSentUri(handler.LastRequest, "patron-password");
        }

        [TestMethod]
        public async Task ExecutePapiAsync_WithSkippedProtectedTokenPreloadAndPlaceholder_FailsBeforeSending()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            var request = new PapiRestRequest($"/protected/v1/1033/100/1/{ProtectedToken.Placeholder}/search/patrons/Boolean");
            var executePapiAsync = typeof(PapiClient)
                .GetMethod("ExecutePapiAsync", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
                .MakeGenericMethod(typeof(PapiResponseCommon));
            var skipMode = Enum.Parse(
                typeof(PapiClient).GetNestedType("ProtectedTokenPreloadMode", System.Reflection.BindingFlags.NonPublic)!,
                "Skip");

            var task = (Task)executePapiAsync.Invoke(client, new object[] { request, CancellationToken.None, skipMode })!;
            var exception = await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () => await task);

            StringAssert.Contains(exception.Message, "ProtectedToken.Placeholder");
            Assert.IsNull(handler.LastRequest);
            Assert.AreEqual($"/protected/v1/1033/100/1/{ProtectedToken.Placeholder}/search/patrons/Boolean", request.Path);
        }

        [TestMethod]
        public async Task BibSearch_RequestShape_IsStable()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            var options = new BibSearchOptions
            {
                Branch = 1,
                SearchType = BibSearchTypes.keyword,
                Qualifier = SearchQualifiers.KW,
                Term = "harry potter & stone",
                SortOption = SearchSortOptions.MP,
                Page = 2,
                PageSize = 15,
                Limit = "3"
            };

            var response = await client.BibSearchAsync(options);

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest!.Method);
            StringAssert.Contains(handler.LastRequest.RequestUri!.AbsolutePath, "/public/v1/1033/100/1/search/bibs/keyword/KW");
            Assert.AreEqual("https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?q=harry%20potter%20%26%20stone&sort=MP&page=2&bibsperpage=15&limit=3", handler.LastRequest.RequestUri.AbsoluteUri);
            var query = ParseQuery(handler.LastRequest.RequestUri.Query);
            Assert.AreEqual("harry potter & stone", query["q"]);
            Assert.AreEqual("MP", query["sort"]);
            Assert.AreEqual("2", query["page"]);
            Assert.AreEqual("15", query["bibsperpage"]);
            Assert.AreEqual("3", query["limit"]);
            Assert.IsNull(handler.LastRequest.Content);
            Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
            AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
        }

        [TestMethod]
        public async Task BibKeywordSearchAsync_ThroughInterface_RequestShapeIsStable()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            IPapiClient client = CreateClient(handler);

            var response = await client.BibKeywordSearchAsync(
                "harry potter & stone",
                branchId: 7,
                page: 2,
                pageSize: 15,
                sortBy: SearchSortOptions.MP);

            Assert.IsNotNull(response);
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest!.Method);
            StringAssert.Contains(handler.LastRequest.RequestUri!.AbsolutePath, "/public/v1/1033/100/7/search/bibs/keyword/KW");
            Assert.AreEqual("https://example.test/PAPIService/REST/public/v1/1033/100/7/search/bibs/keyword/KW?q=harry%20potter%20%26%20stone&sort=MP&page=2&bibsperpage=15", handler.LastRequest.RequestUri.AbsoluteUri);
            var query = ParseQuery(handler.LastRequest.RequestUri.Query);
            Assert.AreEqual("harry potter & stone", query["q"]);
            Assert.AreEqual("MP", query["sort"]);
            Assert.AreEqual("2", query["page"]);
            Assert.AreEqual("15", query["bibsperpage"]);
            Assert.IsNull(handler.LastRequest.Content);
            Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
            AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
        }

        [TestMethod]
        public async Task BibKeywordSearchAsync_ThroughInterfaceWithoutBranchId_UsesOrganizationId()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            IPapiClient client = CreateClient(handler);
            client.OrganizationId = 42;

            var response = await client.BibKeywordSearchAsync("default branch search");

            Assert.IsNotNull(response);
            Assert.IsNotNull(handler.LastRequest);
            StringAssert.Contains(handler.LastRequest!.RequestUri!.AbsolutePath, "/public/v1/1033/100/42/search/bibs/keyword/KW");
            Assert.AreEqual("https://example.test/PAPIService/REST/public/v1/1033/100/42/search/bibs/keyword/KW?q=default%20branch%20search&sort=MP&page=1&bibsperpage=10", handler.LastRequest.RequestUri.AbsoluteUri);
            AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
        }

        [TestMethod]
        public async Task BibBooleanSearchAsync_ThroughInterface_RequestShapeIsStable()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            IPapiClient client = CreateClient(handler);

            var response = await client.BibBooleanSearchAsync(
                "TI=Harry Potter",
                branchId: 8,
                page: 3,
                pageSize: 20,
                sortBy: SearchSortOptions.AU);

            Assert.IsNotNull(response);
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest!.Method);
            StringAssert.Contains(handler.LastRequest.RequestUri!.AbsolutePath, "/public/v1/1033/100/8/search/bibs/boolean");
            Assert.AreEqual("https://example.test/PAPIService/REST/public/v1/1033/100/8/search/bibs/boolean?q=TI%3DHarry%20Potter&sort=AU&page=3&bibsperpage=20", handler.LastRequest.RequestUri.AbsoluteUri);
            var query = ParseQuery(handler.LastRequest.RequestUri.Query);
            Assert.AreEqual("TI=Harry Potter", query["q"]);
            Assert.AreEqual("AU", query["sort"]);
            Assert.AreEqual("3", query["page"]);
            Assert.AreEqual("20", query["bibsperpage"]);
            Assert.IsNull(handler.LastRequest.Content);
            Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
            AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
        }

        [TestMethod]
        public async Task BibBooleanSearchAsync_ThroughInterfaceWithoutBranchId_UsesOrganizationId()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            IPapiClient client = CreateClient(handler);
            client.OrganizationId = 42;

            var response = await client.BibBooleanSearchAsync("TI=Default Branch");

            Assert.IsNotNull(response);
            Assert.IsNotNull(handler.LastRequest);
            StringAssert.Contains(handler.LastRequest!.RequestUri!.AbsolutePath, "/public/v1/1033/100/42/search/bibs/boolean");
            Assert.AreEqual("https://example.test/PAPIService/REST/public/v1/1033/100/42/search/bibs/boolean?q=TI%3DDefault%20Branch&sort=MP&page=1&bibsperpage=10", handler.LastRequest.RequestUri.AbsoluteUri);
            AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
        }

        [TestMethod]
        public async Task BibSearchAsync_DefaultLimit_OmitsLimitAndHashesSentUri()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            var options = new BibSearchOptions
            {
                Branch = 1,
                SearchType = BibSearchTypes.keyword,
                Qualifier = SearchQualifiers.KW,
                Term = "harry potter & stone",
                SortOption = SearchSortOptions.MP,
                Page = 2,
                PageSize = 15
            };

            var response = await client.BibSearchAsync(options);

            Assert.IsNotNull(response);
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual("https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?q=harry%20potter%20%26%20stone&sort=MP&page=2&bibsperpage=15", handler.LastRequest!.RequestUri!.AbsoluteUri);
            var query = ParseQuery(handler.LastRequest.RequestUri.Query);
            Assert.IsFalse(query.ContainsKey("limit"));
            AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_QueryParameterWithNullValue_OmitsParameterAndHashesSentUri()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/search/bibs/keyword/KW");
            request.QueryParameters.Add("q", "harry potter");
            request.QueryParameters.Add("limit", null!);

            await ExecuteRawPapiRequestAsync(client, request);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual("https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?q=harry%20potter", handler.LastRequest!.RequestUri!.AbsoluteUri);
            var query = ParseQuery(handler.LastRequest.RequestUri.Query);
            Assert.IsFalse(query.ContainsKey("limit"));
            AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_QueryParameterWithEmptyStringValue_OmitsParameterAndHashesSentUri()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/search/bibs/keyword/KW");
            request.QueryParameters.Add("q", "harry potter");
            request.QueryParameters.Add("limit", string.Empty);

            await ExecuteRawPapiRequestAsync(client, request);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual("https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?q=harry%20potter", handler.LastRequest!.RequestUri!.AbsoluteUri);
            var query = ParseQuery(handler.LastRequest.RequestUri.Query);
            Assert.IsFalse(query.ContainsKey("limit"));
            AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_QueryParameterWithWhitespaceValue_OmitsParameterAndHashesSentUri()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/search/bibs/keyword/KW");
            request.QueryParameters.Add("q", "harry potter");
            request.QueryParameters.Add("limit", "   ");

            await ExecuteRawPapiRequestAsync(client, request);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual("https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?q=harry%20potter", handler.LastRequest!.RequestUri!.AbsoluteUri);
            var query = ParseQuery(handler.LastRequest.RequestUri.Query);
            Assert.IsFalse(query.ContainsKey("limit"));
            AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_QueryParameterWithBlankKey_OmitsParameterAndHashesSentUri()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/search/bibs/keyword/KW");
            request.QueryParameters.Add("q", "harry potter");
            request.QueryParameters.Add("   ", "branch:1");

            await ExecuteRawPapiRequestAsync(client, request);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual("https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?q=harry%20potter", handler.LastRequest!.RequestUri!.AbsoluteUri);
            var query = ParseQuery(handler.LastRequest.RequestUri.Query);
            Assert.AreEqual(1, query.Count);
            Assert.IsTrue(query.ContainsKey("q"));
            AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_OnlyIneffectiveQueryParameters_OmitsQueryStringAndHashesSentUri()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/search/bibs/keyword/KW");
            request.QueryParameters.Add(" ", "blank key");
            request.QueryParameters.Add("empty", string.Empty);
            request.QueryParameters.Add("null", null!);

            await ExecuteRawPapiRequestAsync(client, request);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual("https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW", handler.LastRequest!.RequestUri!.AbsoluteUri);
            Assert.AreEqual(string.Empty, handler.LastRequest.RequestUri.Query);
            AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_QueryParameters_UseInvariantCultureForSentUriAndHash()
        {
            var originalCulture = CultureInfo.CurrentCulture;
            var originalUiCulture = CultureInfo.CurrentUICulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("fr-FR");
                var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
                var client = CreateClient(handler);
                var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/search/bibs/keyword/KW");
                request.QueryParameters.Add("amount", 12.34m);

                await ExecuteRawPapiRequestAsync(client, request);

                Assert.IsNotNull(handler.LastRequest);
                Assert.AreEqual("https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?amount=12.34", handler.LastRequest!.RequestUri!.AbsoluteUri);
                AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
            }
            finally
            {
                CultureInfo.CurrentCulture = originalCulture;
                CultureInfo.CurrentUICulture = originalUiCulture;
            }
        }

        [TestMethod]
        public async Task ExecutePapiAsync_NonEmptyQueryParameters_HashesSentUri()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/search/bibs/keyword/KW");
            request.QueryParameters.Add("q", "harry potter & stone");
            request.QueryParameters.Add("limit", "branch:1");

            await ExecuteRawPapiRequestAsync(client, request);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual("https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?q=harry%20potter%20%26%20stone&limit=branch%3A1", handler.LastRequest!.RequestUri!.AbsoluteUri);
            AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_ExistingQueryString_AppendsQueryParametersAndHashesSentUri()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/search/bibs/keyword/KW?existing=1");
            request.QueryParameters.Add("q", "harry potter");

            await ExecuteRawPapiRequestAsync(client, request);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual("https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?existing=1&q=harry%20potter", handler.LastRequest!.RequestUri!.AbsoluteUri);
            var query = ParseQuery(handler.LastRequest.RequestUri.Query);
            Assert.AreEqual("1", query["existing"]);
            Assert.AreEqual("harry potter", query["q"]);
            AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
        }

        [TestMethod]
        public async Task AuthenticateStaffUser_RequestShape_IsStable()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0,\"AccessToken\":\"t\",\"AccessSecret\":\"s\",\"AuthExpDate\":\"2030-01-01T00:00:00Z\"}");
            var client = CreateClient(handler);

            var response = await client.AuthenticateStaffUserAsync(new PolarisUser
            {
                Domain = "main",
                Username = "staff",
                Password = "secret"
            });

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpMethod.Post, handler.LastRequest!.Method);
            StringAssert.Contains(handler.LastRequest.RequestUri!.AbsolutePath, "/protected/v1/1033/100/1/authenticator/staff");
            Assert.IsNotNull(handler.LastRequest.Content);
            Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
            Assert.IsNotNull(handler.LastRequestContent);
            StringAssert.Contains(handler.LastRequestContent, "main");
            StringAssert.Contains(handler.LastRequestContent, "staff");
            StringAssert.Contains(handler.LastRequestContent, "secret");
        }

        [TestMethod]
        public async Task PatronUpdate_RequestShape_IsStable()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);

            var response = await client.PatronUpdateAsync("AB C/+#?=", new PatronUpdateParams { EmailAddress = "patron@example.test" }, "1234", ignoresa: true);

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpMethod.Put, handler.LastRequest!.Method);
            var encodedBarcode = WebUtility.UrlEncode("AB C/+#?=");
            StringAssert.Contains(handler.LastRequest.RequestUri!.AbsolutePath, $"/public/v1/1033/100/1/patron/{encodedBarcode}");
            var query = ParseQuery(handler.LastRequest.RequestUri.Query);
            Assert.AreEqual("True", query["ignoresa"]);
            Assert.IsNotNull(handler.LastRequest.Content);
            Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
            Assert.IsNotNull(handler.LastRequestContent);
            StringAssert.Contains(handler.LastRequestContent, "patron@example.test");
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

        [TestMethod]
        public async Task PatronMessages_RequestShape_PreservesBooleanLikeQueryValue()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);

            var response = await client.PatronMessagesGetAsync("ABC 123", unreadOnly: true, password: "1234");

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest!.Method);
            StringAssert.Contains(handler.LastRequest.RequestUri!.AbsolutePath, "/public/v1/1033/100/1/patron/ABC+123/messages");
            var query = ParseQuery(handler.LastRequest.RequestUri.Query);
            Assert.AreEqual("1", query["unreadonly"]);
            Assert.IsNull(handler.LastRequest.Content);
            Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
        }

        [TestMethod]
        public void PreformatRestRequest_HashesQueryParameters_WithOutgoingUriEncoding()
        {
            var client = CreateClient();
            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/search/bibs/keyword/KW");
            request.QueryParameters.Add("q", "harry potter & stone");

            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            var date = formatted.Headers["PolarisDate"].ToString();
            var expectedUri = "https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?q=harry%20potter%20%26%20stone";
            var expectedHash = ComputePapiHash("GET", expectedUri, date, string.Empty, "access-key");
            Assert.AreEqual($"PWS access-id:{expectedHash}", formatted.Headers["Authorization"]);
        }

        private static PapiClient CreateClient(HttpMessageHandler? handler = null)
        {
            var settings = new TestPapiSettings();
            var httpClient = handler == null ? new HttpClient() : new HttpClient(handler);
            return new PapiClient(httpClient, settings)
            {
                AllowStaffOverrideRequests = false,
                UseProtectedTokenCache = false
            };
        }


        private static PapiClient CreateClientWithProtectedCache(HttpMessageHandler handler, string username)
        {
            var client = CreateClient(handler);
            client.AllowStaffOverrideRequests = true;
            client.UseProtectedTokenCache = true;
            client.StaffOverrideAccount = new PolarisUser
            {
                Domain = "main",
                Username = username,
                Password = "secret"
            };

            return client;
        }

        private static async Task ExecuteRawPapiRequestAsync(PapiClient client, PapiRestRequest request)
        {
            var executePapiAsync = typeof(PapiClient)
                .GetMethod("ExecutePapiAsync", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
                .MakeGenericMethod(typeof(PapiResponseCommon));
            var autoMode = Enum.Parse(
                typeof(PapiClient).GetNestedType("ProtectedTokenPreloadMode", System.Reflection.BindingFlags.NonPublic)!,
                "Auto");

            var task = (Task)executePapiAsync.Invoke(client, new object[] { request, CancellationToken.None, autoMode })!;
            await task.ConfigureAwait(false);
        }

        private static void AssertAuthorizationHashesSentUri(HttpRequestMessage request, string password)
        {
            var date = request.Headers.GetValues("PolarisDate").Single();
            var expectedHash = ComputePapiHash(request.Method.Method, request.RequestUri!.AbsoluteUri, date, password, "access-key");
            Assert.AreEqual($"PWS access-id:{expectedHash}", request.Headers.GetValues("Authorization").Single());
        }


        private static Dictionary<string, string> ParseQuery(string query)
        {
            return query.TrimStart('?')
                .Split('&', StringSplitOptions.RemoveEmptyEntries)
                .Select(part => part.Split('=', 2))
                .ToDictionary(parts => WebUtility.UrlDecode(parts[0]), parts => parts.Length > 1 ? WebUtility.UrlDecode(parts[1]) : string.Empty);
        }

        private static string ComputePapiHash(string httpMethod, string uri, string date, string password, string accessKey)
        {
            var hashString = httpMethod + uri + date + password;
            var computedHash = HMACSHA1.HashData(Encoding.UTF8.GetBytes(accessKey), Encoding.UTF8.GetBytes(hashString));
            return Convert.ToBase64String(computedHash);
        }

        private sealed class TestPapiSettings : IPapiSettings
        {
            public string AccessId { get; set; } = "access-id";
            public string AccessKey { get; set; } = "access-key";
            public string Hostname { get; set; } = "https://example.test";
            public int UserId { get; set; } = 1;
            public int WorkstationId { get; set; } = 1;
            public int OrganizationId { get; set; } = 1;
            public PolarisUser? PolarisOverrideAccount { get; set; }
        }

        private sealed class ProtectedTokenCancellationHttpMessageHandler : HttpMessageHandler
        {
            public List<HttpRequestMessage> Requests { get; } = new();
            public List<CancellationToken> CancellationTokens { get; } = new();

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Requests.Add(request);
                CancellationTokens.Add(cancellationToken);
                var requestNumber = Requests.Count;

                var responseJson = requestNumber == 1
                    ? "{\"PAPIErrorCode\":0,\"AccessToken\":\"protected-token\",\"AccessSecret\":\"protected-secret\",\"AuthExpDate\":\"2030-01-01T00:00:00Z\"}"
                    : "{\"PAPIErrorCode\":0}";

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
                });
            }
        }

        private sealed class FailingStaffAuthenticationHttpMessageHandler : HttpMessageHandler
        {
            public int AuthenticationRequestCount { get; private set; }
            public int ProtectedRequestCount { get; private set; }
            public int PublicRequestCount { get; private set; }
            public HttpRequestMessage? LastNonAuthenticationRequest { get; private set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (request.RequestUri!.AbsolutePath.Contains("/authenticator/staff", StringComparison.Ordinal))
                {
                    AuthenticationRequestCount++;
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("{}", Encoding.UTF8, "application/json")
                    });
                }

                LastNonAuthenticationRequest = request;
                if (request.RequestUri!.AbsolutePath.Contains("/protected/", StringComparison.Ordinal))
                {
                    ProtectedRequestCount++;
                }
                else
                {
                    PublicRequestCount++;
                }

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"PAPIErrorCode\":0}", Encoding.UTF8, "application/json")
                });
            }
        }

        private sealed class SequencedProtectedTokenHttpMessageHandler : HttpMessageHandler
        {
            private readonly object _syncRoot = new();
            private int _authenticationRequestCount;

            public int AuthenticationRequestCount => _authenticationRequestCount;
            public List<HttpRequestMessage> ProtectedRequests { get; } = new();

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (request.RequestUri!.AbsolutePath.Contains("/authenticator/staff", StringComparison.Ordinal))
                {
                    var requestNumber = Interlocked.Increment(ref _authenticationRequestCount);
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent($"{{\"PAPIErrorCode\":0,\"AccessToken\":\"protected-token-{requestNumber}\",\"AccessSecret\":\"protected-secret-{requestNumber}\",\"AuthExpDate\":\"2030-01-01T00:00:00Z\"}}", Encoding.UTF8, "application/json")
                    });
                }

                lock (_syncRoot)
                {
                    ProtectedRequests.Add(request);
                }

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"PAPIErrorCode\":0}", Encoding.UTF8, "application/json")
                });
            }
        }

        private sealed class BlockingProtectedTokenHttpMessageHandler : HttpMessageHandler
        {
            private readonly object _syncRoot = new();
            private int _authenticationRequestCount;

            public TaskCompletionSource AuthenticationStarted { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
            public TaskCompletionSource CompleteAuthentication { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
            public int AuthenticationRequestCount => _authenticationRequestCount;
            public List<HttpRequestMessage> ProtectedRequests { get; } = new();

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (request.RequestUri!.AbsolutePath.Contains("/authenticator/staff", StringComparison.Ordinal))
                {
                    Interlocked.Increment(ref _authenticationRequestCount);
                    AuthenticationStarted.TrySetResult();
                    await CompleteAuthentication.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("{\"PAPIErrorCode\":0,\"AccessToken\":\"blocked-token\",\"AccessSecret\":\"blocked-secret\",\"AuthExpDate\":\"2030-01-01T00:00:00Z\"}", Encoding.UTF8, "application/json")
                    };
                }

                lock (_syncRoot)
                {
                    ProtectedRequests.Add(request);
                }

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"PAPIErrorCode\":0}", Encoding.UTF8, "application/json")
                };
            }
        }

        private sealed class ThrowOnSecondEnumerationEnumerable : IEnumerable<int>
        {
            private readonly IEnumerable<int> _ids;

            public int EnumerationCount { get; private set; }

            public ThrowOnSecondEnumerationEnumerable(IEnumerable<int> ids)
            {
                _ids = ids;
            }

            public IEnumerator<int> GetEnumerator()
            {
                EnumerationCount++;
                if (EnumerationCount > 1)
                {
                    throw new InvalidOperationException("The IDs enumerable was enumerated more than once.");
                }

                return _ids.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private sealed class CapturingHttpMessageHandler : HttpMessageHandler
        {
            private readonly string _responseJson;

            public HttpRequestMessage? LastRequest { get; private set; }
            public string? LastRequestContent { get; private set; }
            public CancellationToken LastCancellationToken { get; private set; }

            public CapturingHttpMessageHandler(string responseJson)
            {
                _responseJson = responseJson;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                LastRequest = request;
                LastCancellationToken = cancellationToken;
                LastRequestContent = request.Content == null
                    ? null
                    : await request.Content.ReadAsStringAsync(cancellationToken);

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(_responseJson, Encoding.UTF8, "application/json")
                };
            }
        }
    }
}
