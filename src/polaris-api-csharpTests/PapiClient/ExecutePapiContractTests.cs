using System.Linq;
using System.Reflection;
using Clc.Polaris.Api;
using PapiClientType = Clc.Polaris.Api.PapiClient;
using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Tests;
using Clc.Polaris.Api.Tests.TestInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.PapiClient
{
    [TestClass]
    [UnitCategory]
    public class ExecutePapiContractTests : PapiClientTestBase
    {

        [TestMethod]
        public void ExecutePapiAsync_IsPublicOnPapiClientOnly()
        {
            var papiClientMethod = typeof(PapiClientType)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .SingleOrDefault(method => method.Name == nameof(PapiClientType.ExecutePapiAsync) && method.IsGenericMethodDefinition);
            var interfaceMethod = typeof(IPapiClient)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .SingleOrDefault(method => method.Name == nameof(PapiClientType.ExecutePapiAsync));

            Assert.IsNotNull(papiClientMethod);
            Assert.IsNull(interfaceMethod);
        }


        [TestMethod]
        public void IsStaffAuthenticatorRequest_InvalidOrMissingPath_ReturnsFalse()
        {
            Assert.IsFalse(InvokeIsStaffAuthenticatorRequest(null));
            Assert.IsFalse(InvokeIsStaffAuthenticatorRequest(CreateStaffAuthenticatorRequestWithPath(null)));
            Assert.IsFalse(InvokeIsStaffAuthenticatorRequest(CreateStaffAuthenticatorRequestWithPath(string.Empty)));
            Assert.IsFalse(InvokeIsStaffAuthenticatorRequest(CreateStaffAuthenticatorRequestWithPath("   ")));
        }


        [TestMethod]
        public void IsStaffAuthenticatorRequest_RequiresExactProtectedPostStaffRouteWithoutPlaceholder()
        {
            Assert.IsTrue(InvokeIsStaffAuthenticatorRequest(CreateStaffAuthenticatorRequestWithPath("/protected/v1/1033/100/1/authenticator/staff")));
            Assert.IsFalse(InvokeIsStaffAuthenticatorRequest(PapiRestRequest.Get("/protected/v1/1033/100/1/authenticator/staff")));
            Assert.IsFalse(InvokeIsStaffAuthenticatorRequest(PapiRestRequest.Post("/protected/v1/1033/100/1/authenticator/staff/extra")));
            Assert.IsFalse(InvokeIsStaffAuthenticatorRequest(PapiRestRequest.Post($"/protected/v1/1033/100/1/{ProtectedToken.Placeholder}/authenticator/staff")));
        }
        private static bool InvokeIsStaffAuthenticatorRequest(PapiRestRequest? request)
        {
            var isStaffAuthenticatorRequest = typeof(PapiClientType)
                .GetMethod("IsStaffAuthenticatorRequest", BindingFlags.Static | BindingFlags.NonPublic)!;

            return (bool)isStaffAuthenticatorRequest.Invoke(null, new object?[] { request })!;
        }

        private static PapiRestRequest CreateStaffAuthenticatorRequestWithPath(string? path)
        {
            var request = PapiRestRequest.Post("/protected/v1/1033/100/1/authenticator/staff");
            request.Path = path!;

            return request;
        }

    }
}
