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
    [DoNotParallelize]
    [UnitCategory]
    public class ProtectedTokenCacheTests : PapiClientTokenTestBase
    {
                [TestMethod]
                public void BuildProtectedTokenCacheKey_UsesCredentialFingerprintWithoutRawSecrets()
                {
                    var client = CreateProtectedClient(new ProtectedTokenHttpMessageHandler());
                    client.AccessKey = "raw-access-key-secret";
                    client.StaffOverrideAccount = CreateStaffUser(password: "correct-1");
        
                    var cacheKey = BuildCacheKey(client);
        
                    Assert.IsNotNull(cacheKey);
                    Assert.IsFalse(cacheKey!.Contains(client.StaffOverrideAccount.Password, StringComparison.Ordinal), "Cache key exposed secret material.");
                    Assert.IsFalse(cacheKey.Contains(client.AccessKey, StringComparison.Ordinal), "Cache key exposed secret material.");
                    StringAssert.Contains(cacheKey, client.Hostname.Trim());
                    StringAssert.Contains(cacheKey, client.AccessID.Trim());
                    StringAssert.Contains(cacheKey, client.StaffOverrideAccount.Domain.Trim());
                    StringAssert.Contains(cacheKey, client.StaffOverrideAccount.Username.Trim());
                }
    }
}
