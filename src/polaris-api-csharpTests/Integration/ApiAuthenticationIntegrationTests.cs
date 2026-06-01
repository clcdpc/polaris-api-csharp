using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

[TestClass]
[TestCategory("Integration")]
public sealed class ApiAuthenticationIntegrationTests : IntegrationTestBase
{
    [TestMethod]
    public async Task ApiKeyValidateAsync_WithConfiguredCredentials_ReturnsSuccess()
    {
        RequirePapiConfiguration();

        var response = await Papi.ApiKeyValidateAsync();

        PapiIntegrationAssert.Success(response);
        PapiIntegrationAssert.RequestUriContains(response, "/apikeyvalidate");
    }

    [TestMethod]
    public async Task ApiVersionGetAsync_WithConfiguredCredentials_ReturnsVersion()
    {
        RequirePapiConfiguration();

        var response = await Papi.ApiVersionGetAsync();
        var data = PapiIntegrationAssert.Success(response);

        Assert.IsFalse(string.IsNullOrWhiteSpace(data.Version));
        PapiIntegrationAssert.RequestUriContains(response, "/api");
    }

    [TestMethod]
    public async Task AuthenticatePatronAsync_WithConfiguredPatron_ReturnsPatronToken()
    {
        RequirePatronCredentials();

        var response = await Papi.AuthenticatePatronAsync(Settings.PatronBarcode, Settings.PatronPin);
        var data = PapiIntegrationAssert.Success(response);

        Assert.IsFalse(string.IsNullOrWhiteSpace(data.AccessToken));
    }

    [TestMethod]
    public async Task AuthenticateStaffUserAsync_WhenStaffProtectedTestsEnabled_ReturnsProtectedToken()
    {
        RequireStaffProtectedTestsEnabled();

        var response = await Papi.AuthenticateStaffUserAsync(PapiSettings.PolarisOverrideAccount!);
        var data = PapiIntegrationAssert.Success(response);

        Assert.IsFalse(string.IsNullOrWhiteSpace(data.AccessSecret));
        Assert.IsFalse(string.IsNullOrWhiteSpace(data.AccessToken));
    }

    [TestMethod]
    public void AuthenticatePatronAsync_FailedLogin_IsDisabledByDefaultToAvoidAccountLockout()
    {
        if (!Options.EnableAuthenticationFailureTests)
        {
            Assert.Inconclusive("Failed-authentication tests are disabled by default because repeated bad PIN/password attempts can lock real accounts. Set IntegrationTestOptions:EnableAuthenticationFailureTests=true only for disposable credentials, then add a local test case.");
        }

        Assert.Inconclusive("No failed-authentication scenario is checked in. Configure disposable credentials locally before adding assertions.");
    }
}
