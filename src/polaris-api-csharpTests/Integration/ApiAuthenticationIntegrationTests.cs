using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

[TestClass]
[TestCategory("Integration")]
public sealed class ApiAuthenticationIntegrationTests : PapiIntegrationTestBase
{
    [TestMethod]
    public async Task ApiKeyValidate_ReturnsSuccess()
    {
        var response = await Client.ApiKeyValidateAsync();
        PapiIntegrationAssert.Success(response);
    }

    [TestMethod]
    public async Task ApiVersionGet_ReturnsSuccessAndVersionPayload()
    {
        var response = await Client.ApiVersionGetAsync();
        var data = PapiIntegrationAssert.Success(response);
        Assert.IsFalse(string.IsNullOrWhiteSpace(data.ToString()));
    }

    [TestMethod]
    public async Task AuthenticatePatron_WithConfiguredPatron_ReturnsPatronId()
    {
        RequirePatronCredentials();
        RequirePatronId();

        var response = await Client.AuthenticatePatronAsync(Settings.PatronBarcode, Settings.PatronPin);
        var data = PapiIntegrationAssert.Success(response);
        Assert.AreEqual(Settings.PatronId, data.PatronID);
    }

    [TestMethod]
    public async Task AuthenticateStaffUser_WhenEnabled_ReturnsProtectedToken()
    {
        RequireStaffProtectedTestsEnabled();

        var response = await Client.AuthenticateStaffUserAsync(Client.StaffOverrideAccount!);
        var data = PapiIntegrationAssert.Success(response);
        Assert.IsFalse(string.IsNullOrWhiteSpace(data.AccessSecret));
        Assert.IsFalse(string.IsNullOrWhiteSpace(data.AccessToken));
    }

    [TestMethod]
    public void AuthenticatePatron_FailureScenario_IsDisabledByDefault()
    {
        RequireAuthenticationFailureTestsEnabled();
        Assert.Inconclusive("Configure an explicit disposable patron/password failure scenario before enabling this test; repeated bad PIN attempts can lock real accounts.");
    }
}
