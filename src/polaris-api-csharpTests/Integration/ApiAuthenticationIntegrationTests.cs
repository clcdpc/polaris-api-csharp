using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Tests.Integration.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

[TestClass]
[TestCategory("Integration")]
public sealed class ApiAuthenticationIntegrationTests : PapiIntegrationTestBase
{
    [TestMethod]
    public async Task ApiKeyValidate_ReturnsSuccess()
    {
        var response = await Papi.ApiKeyValidateAsync();
        PapiAssert.Success(response);
    }

    [TestMethod]
    public async Task ApiVersionGet_ReturnsVersionPayload()
    {
        var response = await Papi.ApiVersionGetAsync();
        var data = PapiAssert.Success(response);
        Assert.IsFalse(string.IsNullOrWhiteSpace(data.Version), "Expected API version text.");
    }

    [TestMethod]
    public async Task AuthenticatePatron_WithConfiguredPatron_ReturnsToken()
    {
        RequirePatronCredentials();

        var response = await Papi.AuthenticatePatronAsync(Settings.PatronBarcode, Settings.PatronPin);
        var data = PapiAssert.Success(response);
        Assert.IsFalse(string.IsNullOrWhiteSpace(data.AccessToken), "Expected a patron access token.");
        Assert.IsFalse(string.IsNullOrWhiteSpace(data.AccessSecret), "Expected a patron access secret.");
    }

    [TestMethod]
    public void AuthenticatePatron_FailureScenario_IsDisabledByDefault()
    {
        RequireAuthenticationFailureTestsEnabled();
        Assert.Inconclusive("TODO AuthenticatePatron failed-login coverage: configure an explicit disposable patron credential fixture and assert documented PAPI authentication failure without risking account lockout.");
    }

    [TestMethod]
    public async Task AuthenticateStaffUser_WithConfiguredStaffOverride_ReturnsProtectedToken()
    {
        RequireStaffProtectedTestsEnabled();

        var response = await Papi.AuthenticateStaffUserAsync(Papi.StaffOverrideAccount!);
        var data = PapiAssert.Success(response);
        Assert.IsFalse(string.IsNullOrWhiteSpace(data.AccessToken), "Expected a protected access token.");
        Assert.IsFalse(string.IsNullOrWhiteSpace(data.AccessSecret), "Expected a protected access secret.");
    }
}
