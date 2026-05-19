using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Clc.Polaris.Api;
using Clc.Polaris.Api.Configuration;
using Clc.Polaris.Api.Models;
using Clc.Polaris.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    [TestCategory("Unit")]
    public class NewEndpointsTests
    {
        private sealed class CaptureHttpMessageHandler : HttpMessageHandler
        {
            public HttpRequestMessage? LastRequest { get; private set; }
            public string? RequestContent { get; private set; }
            private readonly string _responseJson;

            public CaptureHttpMessageHandler(string responseJson = "{\"PAPIErrorCode\":0, \"ErrorMessage\":\"\"}")
            {
                _responseJson = responseJson;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                LastRequest = request;
                if (request.Content != null)
                {
                    RequestContent = await request.Content.ReadAsStringAsync(cancellationToken);
                }
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(_responseJson, Encoding.UTF8, "application/json")
                };
                return response;
            }
        }

        private static PapiClient CreateClient(CaptureHttpMessageHandler handler)
        {
            var settings = new PapiSettings
            {
                AccessId = "test-access-id",
                AccessKey = "test-access-key",
                Hostname = "https://example.test",
                OrganizationId = 1,
                UserId = 123,
                WorkstationId = 456
            };

            var httpClient = new HttpClient(handler);
            var client = new PapiClient(httpClient, settings);
            client.Token = new ProtectedToken
            {
                AccessToken = "mock-token",
                AccessSecret = "mock-secret",
                ExpirationDate = DateTime.Now.AddHours(1)
            };
            return client;
        }

        [TestMethod]
        public void ILLRequestCreate_SendsCorrectPostRequest()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);
            var data = new ILLRequestCreateData { PatronID = 123, Title = "Test Title" };

            client.ILLRequestCreate(data);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Post, handler.LastRequest.Method);
            Assert.AreEqual("/PAPIService/REST/public/v1/1033/100/1/illrequest", handler.LastRequest.RequestUri!.AbsolutePath);
        }

        [TestMethod]
        public void ILLRequestCancel_SendsCorrectPutRequest()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);

            client.ILLRequestCancel("barcode123", 987, userId: 1, workstationId: 2);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Put, handler.LastRequest.Method);
            Assert.AreEqual("/PAPIService/REST/public/v1/1033/100/1/patron/barcode123/illrequests/987/cancelled", handler.LastRequest.RequestUri!.AbsolutePath);
            Assert.AreEqual("?wsid=2&userid=1", handler.LastRequest.RequestUri.Query);
        }

        [TestMethod]
        public void ILLRequestCancelAll_SendsCorrectPutRequest()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);

            client.ILLRequestCancelAll("barcode123", userId: 1, workstationId: 2);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Put, handler.LastRequest.Method);
            Assert.AreEqual("/PAPIService/REST/public/v1/1033/100/1/patron/barcode123/illrequests/0/cancelled", handler.LastRequest.RequestUri!.AbsolutePath);
            Assert.AreEqual("?wsid=2&userid=1", handler.LastRequest.RequestUri.Query);
        }

        [TestMethod]
        public void ItemCheckin_SendsCorrectPostRequest()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);
            var data = new ItemCheckInData { LogonBranchID = 1 };

            client.ItemCheckin("itembarcode", data);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Post, handler.LastRequest.Method);
            Assert.AreEqual("/PAPIService/REST/protected/v1/1033/100/1/mock-token/item/itembarcode/checkin", handler.LastRequest.RequestUri!.AbsolutePath);
        }

        [TestMethod]
        public void ItemCheckout_SendsCorrectPostRequest()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);
            var data = new ItemCheckoutData { ItemBarcode = "barcode" };

            client.ItemCheckout("patronbarcode", data);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Post, handler.LastRequest.Method);
            Assert.AreEqual("/PAPIService/REST/public/v1/1033/100/1/patron/patronbarcode/itemsout", handler.LastRequest.RequestUri!.AbsolutePath);
        }

        [TestMethod]
        public void BibsImport_SendsCorrectPostRequest()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);

            client.BibsImport("<record></record>", "profile", workstationId: 77);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Post, handler.LastRequest.Method);
            Assert.AreEqual("/PAPIService/REST/protected/v1/1033/100/1/mock-token/bibs", handler.LastRequest.RequestUri!.AbsolutePath);
            Assert.AreEqual("?ImportProfileName=profile&wsid=77", handler.LastRequest.RequestUri.Query);
        }

        [TestMethod]
        public void MultipartGet_SendsCorrectGetRequest()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);

            client.MultipartGet(111, 222, pickupLocId: 333);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest.Method);
            Assert.AreEqual("/PAPIService/REST/public/v1/1033/100/1/bib/111/multiparts", handler.LastRequest.RequestUri!.AbsolutePath);
            Assert.AreEqual("?PatronID=222&PickupLocID=333", handler.LastRequest.RequestUri.Query);
        }

        [TestMethod]
        public void PickupAreasGet_SendsCorrectGetRequest()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);

            client.PickupAreasGet(55);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest.Method);
            Assert.AreEqual("/PAPIService/REST/public/v1/1033/100/55/pickupareas", handler.LastRequest.RequestUri!.AbsolutePath);
        }

        [TestMethod]
        public void SAMobilePhoneCarriersGet_SendsCorrectGetRequest()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);

            client.SAMobilePhoneCarriersGet(55);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest.Method);
            Assert.AreEqual("/PAPIService/REST/protected/v1/1033/100/55/mock-token/sysadmin/mobilephonecarriers", handler.LastRequest.RequestUri!.AbsolutePath);
        }

        [TestMethod]
        public void RequestsUpdateStatus_SendsCorrectPutRequest()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);

            client.RequestsUpdateStatus(1234, "cancel", itemId: 55, denyReasonId: 2);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Put, handler.LastRequest.Method);
            Assert.AreEqual("/PAPIService/REST/protected/v1/1033/100/1/mock-token/circulation/requests/1234/status", handler.LastRequest.RequestUri!.AbsolutePath);
            Assert.AreEqual("?action=cancel&itemid=55&denyreason=2", handler.LastRequest.RequestUri.Query);
        }

        [TestMethod]
        public void BibGetByTypeV2_SendsCorrectGetRequest()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);

            client.BibGetByTypeV2("key1", "barcode", orgId: 55);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest.Method);
            Assert.AreEqual("/PAPIService/REST/public/v2/1033/100/55/bib/key1", handler.LastRequest.RequestUri!.AbsolutePath);
            Assert.AreEqual("?type=barcode", handler.LastRequest.RequestUri.Query);
        }

        [TestMethod]
        public void PatronUpdateV2_SendsCorrectPutRequest()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);
            var data = new PatronRegistrationData();

            client.PatronUpdateV2("patronbarcode", data, "mypass");

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Put, handler.LastRequest.Method);
            Assert.AreEqual("/PAPIService/REST/public/v2/1033/100/1/patron/patronbarcode", handler.LastRequest.RequestUri!.AbsolutePath);
        }

        [TestMethod]
        public void JobsPurchaseOrdersPost_SendsCorrectPostRequest()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);
            var data = new JobsPurchaseOrdersPostData();

            client.JobsPurchaseOrdersPost(data, orgId: 55);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Post, handler.LastRequest.Method);
            Assert.AreEqual("/PAPIService/REST/protected/v1/1033/100/55/mock-token/jobs/purchaseorders", handler.LastRequest.RequestUri!.AbsolutePath);
        }

        [TestMethod]
        public void JobsPurchaseOrdersPreorderValidation_SendsCorrectPutRequest()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);
            var data = new JobsPurchaseOrdersPutData();

            client.JobsPurchaseOrdersPreorderValidation(data, orgId: 55);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Put, handler.LastRequest.Method);
            Assert.AreEqual("/PAPIService/REST/protected/v1/1033/100/55/mock-token/jobs/purchaseorders", handler.LastRequest.RequestUri!.AbsolutePath);
            Assert.AreEqual("?preordervalidation=1", handler.LastRequest.RequestUri.Query);
        }

        [TestMethod]
        public void JobsPurchaseOrdersStatusGet_SendsCorrectGetRequest()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);
            var jobGuid = Guid.NewGuid();

            client.JobsPurchaseOrdersStatusGet(jobGuid, orgId: 55);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest.Method);
            Assert.AreEqual($"/PAPIService/REST/protected/v1/1033/100/55/mock-token/jobs/purchaseorders/{jobGuid}/status", handler.LastRequest.RequestUri!.AbsolutePath);
        }

        [TestMethod]
        public void JobsPurchaseOrdersResultGet_SendsCorrectGetRequest()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);
            var jobGuid = Guid.NewGuid();

            client.JobsPurchaseOrdersResultGet(jobGuid, orgId: 55);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest.Method);
            Assert.AreEqual($"/PAPIService/REST/protected/v1/1033/100/55/mock-token/jobs/purchaseorders/{jobGuid}/result", handler.LastRequest.RequestUri!.AbsolutePath);
        }

        [TestMethod]
        public void SynchDiscovery_DeletedAuths_SendsCorrectRequests()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);

            client.Synch_GetDeletedAuths("2026-05-19");
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest.Method);
            Assert.AreEqual("/PAPIService/REST/protected/v1/1033/100/1/mock-token/synch/auths/deleted", handler.LastRequest.RequestUri!.AbsolutePath);
            Assert.AreEqual("?deletedate=2026-05-19", handler.LastRequest.RequestUri.Query);

            client.Synch_GetDeletedAuthsPaged("2026-05-19", 10, 20);
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest.Method);
            Assert.AreEqual("/PAPIService/REST/protected/v1/1033/100/1/mock-token/synch/auths/deleted/paged", handler.LastRequest.RequestUri!.AbsolutePath);
            Assert.AreEqual("?deletedate=2026-05-19&lastid=10&nrecs=20", handler.LastRequest.RequestUri.Query);
        }

        [TestMethod]
        public void SynchDiscovery_UpdatedAuths_SendsCorrectRequests()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);

            client.Synch_GetUpdatedAuths("2026-05-19");
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest.Method);
            Assert.AreEqual("/PAPIService/REST/protected/v1/1033/100/1/mock-token/synch/auths/updated", handler.LastRequest.RequestUri!.AbsolutePath);
            Assert.AreEqual("?updatedate=2026-05-19", handler.LastRequest.RequestUri.Query);

            client.Synch_GetUpdatedAuthsPaged("2026-05-19", 10, 20);
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest.Method);
            Assert.AreEqual("/PAPIService/REST/protected/v1/1033/100/1/mock-token/synch/auths/updated/paged", handler.LastRequest.RequestUri!.AbsolutePath);
            Assert.AreEqual("?updatedate=2026-05-19&lastid=10&nrecs=20", handler.LastRequest.RequestUri.Query);
        }

        [TestMethod]
        public void SynchDiscovery_DeletedBibs_SendsCorrectRequests()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);

            client.Synch_GetDeletedBibs("2026-05-19");
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest.Method);
            Assert.AreEqual("/PAPIService/REST/protected/v1/1033/100/1/mock-token/synch/bibs/deleted", handler.LastRequest.RequestUri!.AbsolutePath);
            Assert.AreEqual("?deletedate=2026-05-19", handler.LastRequest.RequestUri.Query);

            client.Synch_GetDeletedBibsPaged("2026-05-19", 10, 20);
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest.Method);
            Assert.AreEqual("/PAPIService/REST/protected/v1/1033/100/1/mock-token/synch/bibs/deleted/paged", handler.LastRequest.RequestUri!.AbsolutePath);
            Assert.AreEqual("?deletedate=2026-05-19&lastid=10&nrecs=20", handler.LastRequest.RequestUri.Query);
        }

        [TestMethod]
        public void SynchDiscovery_UpdatedBibs_SendsCorrectRequests()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);

            client.Synch_GetUpdatedBibs("2026-05-19");
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest.Method);
            Assert.AreEqual("/PAPIService/REST/protected/v1/1033/100/1/mock-token/synch/bibs/updated", handler.LastRequest.RequestUri!.AbsolutePath);
            Assert.AreEqual("?updatedate=2026-05-19", handler.LastRequest.RequestUri.Query);

            client.Synch_GetUpdatedBibsPaged("2026-05-19", 10, 20);
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest.Method);
            Assert.AreEqual("/PAPIService/REST/protected/v1/1033/100/1/mock-token/synch/bibs/updated/paged", handler.LastRequest.RequestUri!.AbsolutePath);
            Assert.AreEqual("?updatedate=2026-05-19&lastid=10&nrecs=20", handler.LastRequest.RequestUri.Query);
        }

        [TestMethod]
        public void SynchDiscovery_DeletedItems_SendsCorrectRequests()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);

            client.Synch_GetDeletedItems("2026-05-19");
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest.Method);
            Assert.AreEqual("/PAPIService/REST/protected/v1/1033/100/1/mock-token/synch/items/deleted", handler.LastRequest.RequestUri!.AbsolutePath);
            Assert.AreEqual("?deletedate=2026-05-19", handler.LastRequest.RequestUri.Query);

            client.Synch_GetDeletedItemsPaged("2026-05-19", 10, 20);
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest.Method);
            Assert.AreEqual("/PAPIService/REST/protected/v1/1033/100/1/mock-token/synch/items/deleted/paged", handler.LastRequest.RequestUri!.AbsolutePath);
            Assert.AreEqual("?deletedate=2026-05-19&lastid=10&nrecs=20", handler.LastRequest.RequestUri.Query);
        }

        [TestMethod]
        public void SynchDiscovery_UpdatedItems_SendsCorrectRequests()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);

            client.Synch_GetUpdatedItems("2026-05-19", "2026-05-20");
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest.Method);
            Assert.AreEqual("/PAPIService/REST/protected/v1/1033/100/1/mock-token/synch/items/updated", handler.LastRequest.RequestUri!.AbsolutePath);
            Assert.AreEqual("?updatedate=2026-05-19&enddate=2026-05-20", handler.LastRequest.RequestUri.Query);

            client.Synch_GetUpdatedItemsPaged("2026-05-19", 10, 20);
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest.Method);
            Assert.AreEqual("/PAPIService/REST/protected/v1/1033/100/1/mock-token/synch/items/updated/paged", handler.LastRequest.RequestUri!.AbsolutePath);
            Assert.AreEqual("?updatedate=2026-05-19&lastid=10&nrecs=20", handler.LastRequest.RequestUri.Query);
        }

        [TestMethod]
        public void SynchDiscovery_DeletedPatrons_SendsCorrectRequests()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);

            client.Synch_GetDeletedPatrons("2026-05-19");
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest.Method);
            Assert.AreEqual("/PAPIService/REST/protected/v1/1033/100/1/mock-token/synch/patrons/deleted", handler.LastRequest.RequestUri!.AbsolutePath);
            Assert.AreEqual("?deletedate=2026-05-19", handler.LastRequest.RequestUri.Query);

            client.Synch_GetDeletedPatronsPaged("2026-05-19", 10, 20);
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest.Method);
            Assert.AreEqual("/PAPIService/REST/protected/v1/1033/100/1/mock-token/synch/patrons/deleted/paged", handler.LastRequest.RequestUri!.AbsolutePath);
            Assert.AreEqual("?deletedate=2026-05-19&lastid=10&nrecs=20", handler.LastRequest.RequestUri.Query);
        }
    }
}
