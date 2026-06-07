using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Clc.Polaris.Api;
using Clc.Polaris.Api.Configuration;
using Clc.Polaris.Api.Models;
using Clc.Rest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    public abstract class RestClientMigrationTestBase : RequestShapeTestBase
    {
        protected static PapiClient CreateClient(HttpMessageHandler? handler = null)
        {
            var settings = new TestPapiSettings();
            var httpClient = handler == null ? new HttpClient() : new HttpClient(handler);
            return new PapiClient(httpClient, settings)
            {
                AllowStaffOverrideRequests = false,
                UseProtectedTokenCache = false
            };
        }


        protected static PapiClient CreateClientWithProtectedCache(HttpMessageHandler handler, string username)
        {
            var client = CreateClient(handler);
            client.AllowStaffOverrideRequests = true;
            client.UseProtectedTokenCache = true;
            client.StaffOverrideAccount = new PolarisUser
            {
                Domain = "main",
                Username = username,
                Password = "secret"
            };

            return client;
        }

        protected static async Task ExecuteRawPapiRequestAsync(PapiClient client, PapiRestRequest request)
        {
            await client.ExecutePapiAsync<PapiResponseCommon>(request).ConfigureAwait(false);
        }

        protected static void AssertAuthorizationHashesSentUri(HttpRequestMessage request, string password)
        {
            var date = request.Headers.GetValues("PolarisDate").Single();
            var expectedHash = PapiSignature.ComputeHash("access-key", request.Method.Method, request.RequestUri!.AbsoluteUri, date, password);
            Assert.AreEqual($"PWS access-id:{expectedHash}", request.Headers.GetValues("Authorization").Single());
        }


        protected static Dictionary<string, string> ParseQuery(string query)
        {
            return query.TrimStart('?')
                .Split('&', StringSplitOptions.RemoveEmptyEntries)
                .Select(part => part.Split('=', 2))
                .ToDictionary(parts => WebUtility.UrlDecode(parts[0]), parts => parts.Length > 1 ? WebUtility.UrlDecode(parts[1]) : string.Empty);
        }


        protected sealed class TestPapiSettings : IPapiSettings
        {
            public string AccessId { get; set; } = "access-id";
            public string AccessKey { get; set; } = "access-key";
            public string Hostname { get; set; } = "https://example.test";
            public int UserId { get; set; } = 1;
            public int WorkstationId { get; set; } = 1;
            public int OrganizationId { get; set; } = 1;
            public PolarisUser? PolarisOverrideAccount { get; set; }
        }

        protected sealed class ProtectedTokenCancellationHttpMessageHandler : HttpMessageHandler
        {
            public List<HttpRequestMessage> Requests { get; } = new();
            public List<CancellationToken> CancellationTokens { get; } = new();

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Requests.Add(request);
                CancellationTokens.Add(cancellationToken);
                var requestNumber = Requests.Count;

                var responseJson = requestNumber == 1
                    ? "{\"PAPIErrorCode\":0,\"AccessToken\":\"protected-token\",\"AccessSecret\":\"protected-secret\",\"AuthExpDate\":\"2030-01-01T00:00:00Z\"}"
                    : "{\"PAPIErrorCode\":0}";

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
                });
            }
        }

        protected sealed class FailingStaffAuthenticationHttpMessageHandler : HttpMessageHandler
        {
            public int AuthenticationRequestCount { get; private set; }
            public int ProtectedRequestCount { get; private set; }
            public int PublicRequestCount { get; private set; }
            public HttpRequestMessage? LastNonAuthenticationRequest { get; private set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (request.RequestUri!.AbsolutePath.Contains("/authenticator/staff", StringComparison.Ordinal))
                {
                    AuthenticationRequestCount++;
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("{}", Encoding.UTF8, "application/json")
                    });
                }

                LastNonAuthenticationRequest = request;
                if (request.RequestUri!.AbsolutePath.Contains("/protected/", StringComparison.Ordinal))
                {
                    ProtectedRequestCount++;
                }
                else
                {
                    PublicRequestCount++;
                }

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"PAPIErrorCode\":0}", Encoding.UTF8, "application/json")
                });
            }
        }

        protected sealed class SequencedProtectedTokenHttpMessageHandler : HttpMessageHandler
        {
            private readonly object _syncRoot = new();
            private int _authenticationRequestCount;

            public int AuthenticationRequestCount => _authenticationRequestCount;
            public List<HttpRequestMessage> ProtectedRequests { get; } = new();

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (request.RequestUri!.AbsolutePath.Contains("/authenticator/staff", StringComparison.Ordinal))
                {
                    var requestNumber = Interlocked.Increment(ref _authenticationRequestCount);
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent($"{{\"PAPIErrorCode\":0,\"AccessToken\":\"protected-token-{requestNumber}\",\"AccessSecret\":\"protected-secret-{requestNumber}\",\"AuthExpDate\":\"2030-01-01T00:00:00Z\"}}", Encoding.UTF8, "application/json")
                    });
                }

                lock (_syncRoot)
                {
                    ProtectedRequests.Add(request);
                }

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"PAPIErrorCode\":0}", Encoding.UTF8, "application/json")
                });
            }
        }

        protected sealed class BlockingProtectedTokenHttpMessageHandler : HttpMessageHandler
        {
            private readonly object _syncRoot = new();
            private int _authenticationRequestCount;

            public TaskCompletionSource AuthenticationStarted { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
            public TaskCompletionSource CompleteAuthentication { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
            public int AuthenticationRequestCount => _authenticationRequestCount;
            public List<HttpRequestMessage> ProtectedRequests { get; } = new();

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (request.RequestUri!.AbsolutePath.Contains("/authenticator/staff", StringComparison.Ordinal))
                {
                    Interlocked.Increment(ref _authenticationRequestCount);
                    AuthenticationStarted.TrySetResult();
                    await CompleteAuthentication.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("{\"PAPIErrorCode\":0,\"AccessToken\":\"blocked-token\",\"AccessSecret\":\"blocked-secret\",\"AuthExpDate\":\"2030-01-01T00:00:00Z\"}", Encoding.UTF8, "application/json")
                    };
                }

                lock (_syncRoot)
                {
                    ProtectedRequests.Add(request);
                }

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"PAPIErrorCode\":0}", Encoding.UTF8, "application/json")
                };
            }
        }

        protected sealed class ThrowOnSecondEnumerationEnumerable : IEnumerable<int>
        {
            private readonly IEnumerable<int> _ids;

            public int EnumerationCount { get; private set; }

            public ThrowOnSecondEnumerationEnumerable(IEnumerable<int> ids)
            {
                _ids = ids;
            }

            public IEnumerator<int> GetEnumerator()
            {
                EnumerationCount++;
                if (EnumerationCount > 1)
                {
                    throw new InvalidOperationException("The IDs enumerable was enumerated more than once.");
                }

                return _ids.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
        }

        protected new sealed class CapturingHttpMessageHandler : HttpMessageHandler
        {
            private readonly string _responseJson;

            public HttpRequestMessage? LastRequest { get; private set; }
            public string? LastRequestContent { get; private set; }
            public CancellationToken LastCancellationToken { get; private set; }

            public CapturingHttpMessageHandler(string responseJson)
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
    }
}
