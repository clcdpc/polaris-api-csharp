using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Reflection;
using Clc.Polaris.Api;
using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass()]
    [DoNotParallelize]
    public class PapiClientTokenTests
    {
    [TestInitialize]
    public void TestInitialize()
    {
        ClearProtectedTokenCache();
    }

    [TestMethod]
    public void Token_WhenTokenIsNullAndNoStaffOverrideAccount_ReturnsNullWithoutAuthenticating()
    {
        var handler = new CapturingHttpMessageHandler();
        var client = CreateClient(handler);

        var token = client.Token;

        Assert.IsNull(token);
        Assert.AreEqual(0, handler.RequestCount);
    }

    [TestMethod]
    public void Token_WhenTokenIsNullAndStaffOverrideAccountExistsAndCacheHasValidToken_ReturnsCachedToken()
    {
        var handler = new CapturingHttpMessageHandler();
        var client = CreateClient(handler);
        var staff = CreateStaffUser();
        client.StaffOverrideAccount = staff;
        client.UseProtectedTokenCache = true;
        SetCachedToken(client.Hostname, staff, new ProtectedToken
        {
            AccessToken = "cached-token",
            AccessSecret = "cached-secret",
            ExpirationDate = DateTime.Now.AddHours(1)
        });

        var token = client.Token;

        Assert.IsNotNull(token);
        Assert.AreEqual("cached-token", token.AccessToken);
        Assert.AreEqual("cached-secret", token.AccessSecret);
        Assert.AreEqual(0, handler.RequestCount);
    }

    [TestMethod]
    public void Token_WhenExistingTokenIsNotExpired_ReturnsExistingTokenWithoutAuthenticating()
    {
        var handler = new CapturingHttpMessageHandler();
        var client = CreateClient(handler);
        client.StaffOverrideAccount = CreateStaffUser();
        client.Token = new ProtectedToken
        {
            AccessToken = "existing-token",
            AccessSecret = "existing-secret",
            ExpirationDate = DateTime.Now.AddHours(1)
        };

        var token = client.Token;

        Assert.IsNotNull(token);
        Assert.AreEqual("existing-token", token.AccessToken);
        Assert.AreEqual(0, handler.RequestCount);
    }

    [TestMethod]
    public void Token_WhenExistingTokenIsExpiredAndStaffOverrideAccountExistsAndCacheHasValidToken_UsesCachedToken()
    {
        var handler = new CapturingHttpMessageHandler();
        var client = CreateClient(handler);
        var staff = CreateStaffUser();
        client.StaffOverrideAccount = staff;
        client.UseProtectedTokenCache = true;
        client.Token = new ProtectedToken
        {
            AccessToken = "expired-token",
            AccessSecret = "expired-secret",
            ExpirationDate = DateTime.Now.AddHours(-1)
        };
        SetCachedToken(client.Hostname, staff, new ProtectedToken
        {
            AccessToken = "cached-token",
            AccessSecret = "cached-secret",
            ExpirationDate = DateTime.Now.AddHours(1)
        });

        var token = client.Token;

        Assert.IsNotNull(token);
        Assert.AreEqual("cached-token", token.AccessToken);
        Assert.AreEqual(0, handler.RequestCount);
    }

    [TestMethod]
    public void Token_WhenExistingTokenIsExpiredAndNoStaffOverrideAccount_ReturnsExistingTokenWithoutAuthenticating()
    {
        var handler = new CapturingHttpMessageHandler();
        var client = CreateClient(handler);
        client.Token = new ProtectedToken
        {
            AccessToken = "expired-token",
            AccessSecret = "expired-secret",
            ExpirationDate = DateTime.Now.AddHours(-1)
        };

        var token = client.Token;

        Assert.IsNotNull(token);
        Assert.AreEqual("expired-token", token.AccessToken);
        Assert.AreEqual(0, handler.RequestCount);
    }

    [TestMethod]
    public void Token_WhenCacheEnabledTokenNullAndNoStaffOverrideAccount_ReturnsNullWithoutThrowing()
    {
        var handler = new CapturingHttpMessageHandler();
        var client = CreateClient(handler);
        client.UseProtectedTokenCache = true;
        client.StaffOverrideAccount = null;
        client.Token = null;

        var token = client.Token;

        Assert.IsNull(token);
        Assert.AreEqual(0, handler.RequestCount);
    }

    private static PapiClient CreateClient(CapturingHttpMessageHandler handler)
    {
        var hostname = $"https://example-{Guid.NewGuid():N}.test";
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri($"{hostname}/")
        };

        return new PapiClient(httpClient, null)
        {
            Hostname = hostname,
            AccessID = "access-id",
            AccessKey = "access-key",
            UseProtectedTokenCache = false,
            AllowStaffOverrideRequests = false
        };
    }

    private static PolarisUser CreateStaffUser()
    {
        return new PolarisUser
        {
            Domain = "domain",
            Username = "user",
            Password = "password"
        };
    }

    private static string CreateProtectedTokenJson(string accessToken, string accessSecret, DateTime expirationDate)
    {
        return
            "{" +
            $"\"PAPIErrorCode\":0," +
            $"\"AccessToken\":\"{accessToken}\"," +
            $"\"AccessSecret\":\"{accessSecret}\"," +
            $"\"AuthExpDate\":\"{expirationDate:O}\"" +
            "}";
    }

    private sealed class CapturingHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _responseJson;

        public int RequestCount { get; private set; }
        public HttpRequestMessage? LastRequest { get; private set; }

        public CapturingHttpMessageHandler(string? responseJson = null)
        {
            _responseJson = responseJson ?? CreateProtectedTokenJson("new-token", "new-secret", DateTime.UtcNow.AddHours(1));
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestCount++;
            LastRequest = request;

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_responseJson, Encoding.UTF8, "application/json")
            });
        }
    }

    private static void ClearProtectedTokenCache()
    {
        var cacheProperty = typeof(PapiClient).GetProperty("ProtectedTokenCache", BindingFlags.NonPublic | BindingFlags.Static);
        var cache = cacheProperty?.GetValue(null) as ConcurrentDictionary<string, ProtectedToken>;
        cache?.Clear();
    }

    private static void SetCachedToken(string hostname, PolarisUser staffUser, ProtectedToken token)
    {
        var cacheProperty = typeof(PapiClient).GetProperty("ProtectedTokenCache", BindingFlags.NonPublic | BindingFlags.Static);
        var cache = cacheProperty?.GetValue(null) as ConcurrentDictionary<string, ProtectedToken>;
        cache?.TryAdd($"{hostname}{staffUser.Domain}{staffUser.Username}", token);
    }
}
}
