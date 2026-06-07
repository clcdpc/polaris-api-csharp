using Clc.Polaris.Api;
using Clc.Polaris.Api.Tests;
using Clc.Polaris.Api.Models;
using Clc.Rest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api.Tests.Client
{
    [TestClass]
    [UnitCategory]
    [DoNotParallelize]
    public class TokenPropertyTests : PapiClientTestBase
    {
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
        public void Token_WhenTokenIsNullAndStaffOverrideAccountExistsAndCacheHasValidToken_ReturnsNullWithoutCacheLookupOrAuthenticating()
        {
            var handler = new CapturingHttpMessageHandler();
            var client = CreateClient(handler);
            var staff = CreateStaffUser();
            client.StaffOverrideAccount = staff;
            client.UseProtectedTokenCache = true;
            SetCachedToken(client.Hostname, client.AccessID, client.AccessKey, staff, new ProtectedToken
            {
                AccessToken = "cached-token",
                AccessSecret = "cached-secret",
                ExpirationDate = DateTime.Now.AddHours(1)
            });

            var token = client.Token;

            Assert.IsNull(token);
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
        public void Token_WhenExistingTokenIsExpiredAndStaffOverrideAccountExistsAndCacheHasValidToken_ReturnsNullAndClearsTokenWithoutCacheLookupOrAuthenticating()
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
            SetCachedToken(client.Hostname, client.AccessID, client.AccessKey, staff, new ProtectedToken
            {
                AccessToken = "cached-token",
                AccessSecret = "cached-secret",
                ExpirationDate = DateTime.Now.AddHours(1)
            });

            var token = client.Token;

            Assert.IsNull(token);
            Assert.IsNull(client.Token);
            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public void Token_WhenExistingTokenIsExpiredAndNoStaffOverrideAccount_ReturnsNullAndClearsTokenWithoutAuthenticating()
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

            Assert.IsNull(token);
            Assert.IsNull(client.Token);
            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public void Token_WhenExistingTokenHasNoExpiration_ReturnsNullAndClearsTokenWithoutAuthenticating()
        {
            var handler = new CapturingHttpMessageHandler();
            var client = CreateClient(handler);
            client.Token = new ProtectedToken
            {
                AccessToken = "missing-expiration-token",
                AccessSecret = "missing-expiration-secret"
            };

            var token = client.Token;

            Assert.IsNull(token);
            Assert.IsNull(client.Token);
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
    }
}
