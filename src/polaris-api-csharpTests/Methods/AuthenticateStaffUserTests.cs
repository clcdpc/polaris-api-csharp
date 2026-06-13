namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class AuthenticateStaffUserTests : IntegrationTestBase
    {
        [TestMethod]
        [ProtectedReadOnlyIntegrationTest]
        public async Task AuthenticateStaffUserTest()
        {
            PolarisUser? staffOverrideAccount = Papi.StaffOverrideAccount;
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.AuthenticateStaffUserAsync(staffOverrideAccount!, TestContext.CancellationToken);
            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.AccessSecret));
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.AccessToken));
        }
    }
}
