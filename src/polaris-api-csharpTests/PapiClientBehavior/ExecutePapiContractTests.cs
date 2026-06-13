using System.Reflection;

namespace Clc.Polaris.Api.Tests.PapiClientBehavior
{
    [TestClass]
    [UnitTest]
    public class ExecutePapiContractTests : PapiClientTestBase
    {
        [TestMethod]
        public void ExecutePapiAsync_IsPublicOnPapiClientOnly()
        {
            var papiClientMethod = typeof(PapiClient)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .SingleOrDefault(method => method.Name == nameof(PapiClient.ExecutePapiAsync) && method.IsGenericMethodDefinition);
            var interfaceMethod = typeof(IPapiClient)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .SingleOrDefault(method => method.Name == nameof(PapiClient.ExecutePapiAsync));

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
            return PapiRequestClassifier.IsStaffAuthenticatorRequest(request);
        }

        private static PapiRestRequest CreateStaffAuthenticatorRequestWithPath(string? path)
        {
            var request = PapiRestRequest.Post("/protected/v1/1033/100/1/authenticator/staff");
            request.Path = path!;

            return request;
        }
    }
}
