using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class AuthenticateStaffUserTests : IntegrationTestBase
    {
        [TestMethod]
        [ProtectedReadOnlyIntegrationCategory]
        public async Task AuthenticateStaffUserTest()
        {
            var staffOverrideAccount = Papi.StaffOverrideAccount;
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = await Papi.AuthenticateStaffUserAsync(staffOverrideAccount!);
            Assert.AreEqual(response.Data.PAPIErrorCode, 0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.AccessSecret));
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Data.AccessToken));
        }
    }
}
