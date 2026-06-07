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
    public class ProtectedTokenPatronSearchTests : RestClientMigrationTestBase
    {
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
                    var expectedHash = PapiSignature.ComputeHash("access-key", "GET", expectedUri, date, "hash-secret");
                    var placeholderUri = expectedUri.Replace("hash-token", ProtectedToken.Placeholder, StringComparison.Ordinal);
                    var placeholderHash = PapiSignature.ComputeHash("access-key", "GET", placeholderUri, date, "hash-secret");
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
        
                    StringAssert.Contains(exception.Message, "valid protected access token");
                    StringAssert.Contains(exception.Message, "ProtectedToken.Placeholder");
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
        
                    StringAssert.Contains(exception.Message, "valid protected access token");
                    StringAssert.Contains(exception.Message, "ProtectedToken.Placeholder");
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
    }
}
