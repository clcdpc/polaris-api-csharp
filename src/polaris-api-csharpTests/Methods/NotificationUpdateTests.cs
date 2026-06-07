using System;
using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    public sealed class NotificationUpdateTests : IntegrationTestBase
    {
        [TestMethod()]
        [ProtectedMutatingIntegrationCategory]
        [DoNotParallelize]
        public async Task NotificationUpdateTest()
        {
            IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings);

            var response = (await Papi.NotificationUpdateAsync(new NotificationUpdateParams { PatronId = Settings.PatronId, DeliveryString = "test@test.test", ReportingOrgID = 7, NotificationDeliveryDate = DateTime.Now, DeliveryOptionId = 2, Details = CreateUniqueTestArtifactText(maxLength: 80), NotificationStatusId = NotificationStatus.EmailCompleted, NotificationTypeId = 1 })).Data;
            Assert.IsTrue(response.PAPIErrorCode == -1);
        }
    }
}
