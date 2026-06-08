using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Clc.Polaris.Api;
using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Tests;
using Clc.Polaris.Api.Tests.TestInfrastructure;
using Clc.Rest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.PapiClient
{
    [TestClass]
    [DoNotParallelize]
    [UnitCategory]
    public class ExecutePapiContractTests : PapiClientTestBase
    {
        [TestMethod]
        public void ExecutePapiAsync_IsPublicOnPapiClientOnly()
        {
            var papiClientMethod = typeof(global::Clc.Polaris.Api.PapiClient)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .SingleOrDefault(method => method.Name == nameof(global::Clc.Polaris.Api.PapiClient.ExecutePapiAsync) && method.IsGenericMethodDefinition);
            var interfaceMethod = typeof(IPapiClient)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .SingleOrDefault(method => method.Name == nameof(global::Clc.Polaris.Api.PapiClient.ExecutePapiAsync));

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
    }
}
