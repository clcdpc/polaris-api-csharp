using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Clc.Polaris.Api;
using Clc.Polaris.Api.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    public abstract class RequestShapeTestBase
    {
        protected sealed class CapturingHttpMessageHandler : HttpMessageHandler
        {
            private readonly string _responseJson;

            public HttpRequestMessage? LastRequest { get; private set; }
            public string? LastRequestContent { get; private set; }
            public CancellationToken LastCancellationToken { get; private set; }

            public CapturingHttpMessageHandler(string responseJson = "{\"PAPIErrorCode\":0}")
            {
                _responseJson = responseJson;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                LastRequest = request;
                LastCancellationToken = cancellationToken;
                LastRequestContent = request.Content == null
                    ? null
                    : await request.Content.ReadAsStringAsync(cancellationToken);

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(_responseJson, Encoding.UTF8, "application/json")
                };
            }
        }

        protected static PapiClient CreateRequestShapeClient(CapturingHttpMessageHandler handler)
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
            return new PapiClient(httpClient, settings);
        }
    }
}
