using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Tests;
using Clc.Polaris.Api.Tests.TestInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Methods.ProtectedTokens
{
    [TestClass]
    [DoNotParallelize]
    [UnitTest]
    public class PatronAccountGetTests : PapiClientTestBase
    {
        [TestInitialize]
        public void TestInitialize()
        {
            ClearProtectedTokenState();
        }

        [TestMethod]
        public async Task PatronAccountGetAsync_FailedStaffAuthentication_DoesNotPopulateProtectedTokenCache()
        {
            var handler = new ProtectedTokenHttpMessageHandler(HttpStatusCode.InternalServerError, CreatePapiResponseJson(1));
            var client = CreateProtectedClient(handler);

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => client.PatronAccountGetAsync("ABC123", cancellationToken: TestContext.CancellationToken));

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(0, handler.NonAuthenticationRequestCount);
            Assert.IsNull(client.Token);
            Assert.IsFalse(TryGetCachedToken(client.Hostname, client.AccessID, client.AccessKey, client.StaffOverrideAccount, out _));
        }

        [TestMethod]
        public async Task PatronAccountGetAsync_NullDataStaffAuthentication_DoesNotPopulateProtectedTokenCache()
        {
            var handler = new ProtectedTokenHttpMessageHandler(HttpStatusCode.OK, CreateEmptyJsonObject());
            var client = CreateProtectedClient(handler);

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => client.PatronAccountGetAsync("ABC123", cancellationToken: TestContext.CancellationToken));

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(0, handler.NonAuthenticationRequestCount);
            Assert.IsNull(client.Token);
            Assert.IsFalse(TryGetCachedToken(client.Hostname, client.AccessID, client.AccessKey, client.StaffOverrideAccount, out _));
        }

        [TestMethod]
        public async Task PatronAccountGetAsync_FailedStaffAuthentication_AfterExpiredExistingTokenClearsToken()
        {
            var handler = new ProtectedTokenHttpMessageHandler(HttpStatusCode.InternalServerError, CreatePapiResponseJson(1));
            var client = CreateProtectedClient(handler);
            client.Token = new ProtectedToken
            {
                AccessToken = "existing-token",
                AccessSecret = "existing-secret",
                ExpirationDate = ExpiredProtectedTokenExpirationDate
            };

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => client.PatronAccountGetAsync("ABC123", cancellationToken: TestContext.CancellationToken));

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(0, handler.NonAuthenticationRequestCount);
            Assert.IsNull(client.Token);
            Assert.IsFalse(TryGetCachedToken(client.Hostname, client.AccessID, client.AccessKey, client.StaffOverrideAccount, out _));
        }

        [TestMethod]
        public async Task PatronAccountGetAsync_NullDataStaffAuthentication_AfterExpiredExistingTokenClearsToken()
        {
            var handler = new ProtectedTokenHttpMessageHandler(HttpStatusCode.OK, CreateEmptyJsonObject());
            var client = CreateProtectedClient(handler);
            client.Token = new ProtectedToken
            {
                AccessToken = "existing-token",
                AccessSecret = "existing-secret",
                ExpirationDate = ExpiredProtectedTokenExpirationDate
            };

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => client.PatronAccountGetAsync("ABC123", cancellationToken: TestContext.CancellationToken));

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(0, handler.NonAuthenticationRequestCount);
            Assert.IsNull(client.Token);
            Assert.IsFalse(TryGetCachedToken(client.Hostname, client.AccessID, client.AccessKey, client.StaffOverrideAccount, out _));
        }

        [TestMethod]
        public async Task PatronAccountGetAsync_InvalidStaffAuthentication_AfterExpiredExistingTokenClearsTokenAndDoesNotCache()
        {
            var handler = new ProtectedTokenHttpMessageHandler(HttpStatusCode.OK, CreateProtectedTokenJson(accessToken: string.Empty, accessSecret: string.Empty, expirationDate: ValidProtectedTokenExpirationDate));
            var client = CreateProtectedClient(handler);
            client.Token = new ProtectedToken
            {
                AccessToken = "existing-token",
                AccessSecret = "existing-secret",
                ExpirationDate = ExpiredProtectedTokenExpirationDate
            };

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => client.PatronAccountGetAsync("ABC123", cancellationToken: TestContext.CancellationToken));

            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(0, handler.NonAuthenticationRequestCount);
            Assert.IsNull(client.Token);
            Assert.IsFalse(TryGetCachedToken(client.Hostname, client.AccessID, client.AccessKey, client.StaffOverrideAccount, out _));
        }

        [TestMethod]
        public async Task PatronAccountGetAsync_PublicStaffOverride_FailedStaffAuthentication_ThrowsBeforeFinalRequest()
        {
            var handler = new ProtectedTokenHttpMessageHandler(HttpStatusCode.InternalServerError, CreatePapiResponseJson(1));
            var client = CreateProtectedClient(handler);

            var exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => client.PatronAccountGetAsync("ABC123", cancellationToken: TestContext.CancellationToken));

            Assert.Contains("Staff authentication did not succeed", exception.Message);
            Assert.AreEqual(1, handler.AuthenticationRequestCount);
            Assert.AreEqual(0, handler.NonAuthenticationRequestCount);
        }

        [TestMethod]
        public async Task PatronAccountGetAsync_PublicRequestWithoutStaffOverrideAccount_ContinuesAsOrdinaryPublicRequest()
        {
            var handler = new ProtectedTokenHttpMessageHandler(HttpStatusCode.InternalServerError, CreatePapiResponseJson(1));
            var client = CreateProtectedClient(handler);
            client.StaffOverrideAccount = null;
            client.AllowStaffOverrideRequests = true;

            var response = await client.PatronAccountGetAsync("ABC123", cancellationToken: TestContext.CancellationToken);

            Assert.IsNotNull(response);
            Assert.AreEqual(0, handler.AuthenticationRequestCount);
            Assert.AreEqual(1, handler.NonAuthenticationRequestCount);
            Assert.Contains("/public/v1/1033/100/1/patron/ABC123/account/outstanding", handler.RequestPaths.Single());
        }

        [TestMethod]
        public async Task PatronAccountGetAsync_PublicRequestWithExplicitPassword_DoesNotRequireStaffOverrideToken()
        {
            var handler = new ProtectedTokenHttpMessageHandler(HttpStatusCode.InternalServerError, CreatePapiResponseJson(1));
            var client = CreateProtectedClient(handler);

            var response = await client.PatronAccountGetAsync("ABC123", password: "patron-password", TestContext.CancellationToken);

            Assert.IsNotNull(response);
            Assert.AreEqual(0, handler.AuthenticationRequestCount);
            Assert.AreEqual(1, handler.NonAuthenticationRequestCount);
            Assert.Contains("/public/v1/1033/100/1/patron/ABC123/account/outstanding", handler.RequestPaths.Single());
        }
    }
}
