using System.Net.Http;
using System.Threading.Tasks;
using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Tests;
using Clc.Polaris.Api.Tests.TestInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Methods.RequestShape
{
    [TestClass]
    [UnitCategory]
    public class AuthenticateStaffUserTests : PapiClientTestBase
    {

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
    }
}
