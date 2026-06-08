using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Clc.Polaris.Api;
using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Tests;
using Clc.Polaris.Api.Tests.TestInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Methods.RequestShape
{
    [TestClass]
    [UnitCategory]
    public class PatronReadingHistoryClearTests : PapiClientTestBase
    {
        [TestMethod]
        public async Task PatronReadingHistoryClearAsync_EnumeratesIdsOnceAndSendsCommaSeparatedIds()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            var ids = new ThrowOnSecondEnumerationEnumerable(new[] { 101, 202, 303 });

            var response = await client.PatronReadingHistoryClearAsync("ABC123", "patron-password", ids);

            Assert.IsNotNull(response);
            Assert.AreEqual(1, ids.EnumerationCount);
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Delete, handler.LastRequest!.Method);
            var query = ParseQuery(handler.LastRequest.RequestUri!.Query);
            Assert.AreEqual("101,202,303", query["ids"]);
            AssertAuthorizationHashesSentUri(handler.LastRequest, "patron-password");
        }

        private sealed class ThrowOnSecondEnumerationEnumerable : IEnumerable<int>
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

    }
}
