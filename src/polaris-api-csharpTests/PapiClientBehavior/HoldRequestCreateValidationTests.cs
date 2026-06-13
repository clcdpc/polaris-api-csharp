using System;
using System.Threading.Tasks;
using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Tests.TestInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.PapiClientBehavior
{
    [TestClass]
    [UnitTest]
    public sealed class HoldRequestCreateValidationTests : PapiClientTestBase
    {
        [TestMethod]
        public async Task HoldRequestCreateAsync_NullContextIds_UsesPapiClientDefaultsInBodyAndUrl()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);
            client.OrganizationId = 123;
            client.UserId = 456;
            client.WorkstationId = 789;

            var holdParams = new HoldRequestCreateParams
            {
                PatronID = 1,
                BibID = 2,
                PickupOrgID = 0,
                WorkstationID = null,
                UserID = null,
                RequestingOrgID = null
            };

            await client.HoldRequestCreateAsync(holdParams, cancellationToken: TestContext.CancellationToken);

            AssertLastRequestPathContains(handler, "/public/v1/1033/100/123/holdrequest");
            AssertLastRequestBodyJsonPropertyValue(handler, nameof(HoldRequestCreateParams.WorkstationID), 789);
            AssertLastRequestBodyJsonPropertyValue(handler, nameof(HoldRequestCreateParams.UserID), 456);
            AssertLastRequestBodyJsonPropertyValue(handler, nameof(HoldRequestCreateParams.RequestingOrgID), 123);

            Assert.AreEqual(789, holdParams.WorkstationID);
            Assert.AreEqual(456, holdParams.UserID);
            Assert.AreEqual(123, holdParams.RequestingOrgID);
        }

        [TestMethod]
        public async Task HoldRequestCreateAsync_ExplicitContextIds_UsesProvidedValuesInBodyAndUrl()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);
            client.OrganizationId = 123;
            client.UserId = 456;
            client.WorkstationId = 789;

            var holdParams = new HoldRequestCreateParams
            {
                PatronID = 1,
                BibID = 2,
                PickupOrgID = 0,
                WorkstationID = 10,
                UserID = 20,
                RequestingOrgID = 30
            };

            await client.HoldRequestCreateAsync(holdParams, cancellationToken: TestContext.CancellationToken);

            AssertLastRequestPathContains(handler, "/public/v1/1033/100/30/holdrequest");
            AssertLastRequestBodyJsonPropertyValue(handler, nameof(HoldRequestCreateParams.WorkstationID), 10);
            AssertLastRequestBodyJsonPropertyValue(handler, nameof(HoldRequestCreateParams.UserID), 20);
            AssertLastRequestBodyJsonPropertyValue(handler, nameof(HoldRequestCreateParams.RequestingOrgID), 30);

            Assert.AreEqual(10, holdParams.WorkstationID);
            Assert.AreEqual(20, holdParams.UserID);
            Assert.AreEqual(30, holdParams.RequestingOrgID);
        }

        [TestMethod]
        public async Task HoldRequestCreateAsync_ZeroContextId_ThrowsArgumentOutOfRangeException()
        {
            var client = CreateClient();

            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () =>
                await client.HoldRequestCreateAsync(new HoldRequestCreateParams
                {
                    PatronID = 1,
                    BibID = 2,
                    PickupOrgID = 0,
                    WorkstationID = 0
                }, cancellationToken: TestContext.CancellationToken));

            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () =>
                await client.HoldRequestCreateAsync(new HoldRequestCreateParams
                {
                    PatronID = 1,
                    BibID = 2,
                    PickupOrgID = 0,
                    UserID = 0
                }, cancellationToken: TestContext.CancellationToken));

            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () =>
                await client.HoldRequestCreateAsync(new HoldRequestCreateParams
                {
                    PatronID = 1,
                    BibID = 2,
                    PickupOrgID = 0,
                    RequestingOrgID = 0
                }, cancellationToken: TestContext.CancellationToken));
        }

        [TestMethod]
        public async Task HoldRequestCreateAsync_NegativePickupOrgId_ThrowsArgumentOutOfRangeException()
        {
            var client = CreateClient();

            var holdParams = new HoldRequestCreateParams
            {
                PatronID = 1,
                BibID = 2,
                PickupOrgID = -1
            };

            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () =>
                await client.HoldRequestCreateAsync(holdParams, cancellationToken: TestContext.CancellationToken));
        }

        [TestMethod]
        public async Task HoldRequestCreateAsync_ZeroPickupOrgId_DoesNotFailValidation()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);

            var holdParams = new HoldRequestCreateParams
            {
                PatronID = 1,
                BibID = 2,
                PickupOrgID = 0
            };

            await client.HoldRequestCreateAsync(holdParams, cancellationToken: TestContext.CancellationToken);

            AssertLastRequestBodyJsonPropertyValue(handler, nameof(HoldRequestCreateParams.PickupOrgID), 0);
        }
    }
}
