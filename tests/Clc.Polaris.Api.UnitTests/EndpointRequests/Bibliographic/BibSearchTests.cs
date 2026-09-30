namespace Clc.Polaris.Api.UnitTests.EndpointRequests.Bibliographic
{
    [TestClass]
    [UnitTest]
    public class BibSearchTests : PapiClientUnitTestBase
    {
        [TestMethod]
        public async Task BibSearch_RequestShape_IsStable()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);
            var options = new BibSearchOptions
            {
                Branch = 1,
                SearchType = BibSearchTypes.keyword,
                Qualifier = SearchQualifiers.KW,
                Term = "harry potter & stone",
                SortOption = SearchSortOptions.MP,
                Page = 2,
                PageSize = 15,
                Limit = "3"
            };

            var response = await client.BibSearchAsync(options, TestContext.CancellationToken);

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest!.Method);
            Assert.Contains("/public/v1/1033/100/1/search/bibs/keyword/KW", handler.LastRequest.RequestUri!.AbsolutePath);
            var expectedUri = "https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?q=harry%20potter%20%26%20stone&sortby=MP&page=2&bibsperpage=15&limit=3";
            Assert.AreEqual(expectedUri, handler.LastRequest.RequestUri.AbsoluteUri);
            var query = ParseQuery(handler.LastRequest.RequestUri.Query);
            Assert.AreEqual("harry potter & stone", query["q"]);
            Assert.AreEqual("MP", query["sortby"]);
            Assert.AreEqual("2", query["page"]);
            Assert.AreEqual("15", query["bibsperpage"]);
            Assert.AreEqual("3", query["limit"]);
            Assert.IsNull(handler.LastRequest.Content);
            Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
            AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
        }


        [TestMethod]
        public async Task BibSearchAsync_DefaultBranch_UsesConfiguredOrganizationId()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);
            client.OrganizationId = 101;
            var options = new BibSearchOptions { Term = "configured branch" };

            await client.BibSearchAsync(options, TestContext.CancellationToken);

            Assert.Contains("/public/v1/1033/100/101/search/bibs/keyword/KW", handler.LastRequest!.RequestUri!.AbsolutePath);
        }

        [TestMethod]
        public async Task BibSearchAsync_ExplicitBranch_PreservesSuppliedValue()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);
            client.OrganizationId = 101;
            var options = new BibSearchOptions { Term = "explicit branch", Branch = 77 };

            await client.BibSearchAsync(options, TestContext.CancellationToken);

            Assert.Contains("/public/v1/1033/100/77/search/bibs/keyword/KW", handler.LastRequest!.RequestUri!.AbsolutePath);
        }

        [TestMethod]
        public async Task BibKeywordSearchAsync_ThroughInterface_RequestShapeIsStable()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            IPapiClient client = CreateClient(handler);

            var response = await client.BibKeywordSearchAsync("harry potter & stone", branchId: 7, page: 2, pageSize: 15, sortBy: SearchSortOptions.MP, TestContext.CancellationToken);

            Assert.IsNotNull(response);
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest!.Method);
            Assert.Contains("/public/v1/1033/100/7/search/bibs/keyword/KW", handler.LastRequest.RequestUri!.AbsolutePath);
            var expectedUri = "https://example.test/PAPIService/REST/public/v1/1033/100/7/search/bibs/keyword/KW?q=harry%20potter%20%26%20stone&sortby=MP&page=2&bibsperpage=15";
            Assert.AreEqual(expectedUri, handler.LastRequest.RequestUri.AbsoluteUri);
            var query = ParseQuery(handler.LastRequest.RequestUri.Query);
            Assert.AreEqual("harry potter & stone", query["q"]);
            Assert.AreEqual("MP", query["sortby"]);
            Assert.AreEqual("2", query["page"]);
            Assert.AreEqual("15", query["bibsperpage"]);
            Assert.IsNull(handler.LastRequest.Content);
            Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
            AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
        }

        [TestMethod]
        public async Task BibKeywordSearchAsync_ThroughInterfaceWithoutBranchId_UsesOrganizationId()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);
            client.OrganizationId = 42;

            var response = await client.BibKeywordSearchAsync("default branch search", cancellationToken: TestContext.CancellationToken);

            Assert.IsNotNull(response);
            Assert.IsNotNull(handler.LastRequest);
            Assert.Contains("/public/v1/1033/100/42/search/bibs/keyword/KW", handler.LastRequest!.RequestUri!.AbsolutePath);
            var expectedUri = "https://example.test/PAPIService/REST/public/v1/1033/100/42/search/bibs/keyword/KW?q=default%20branch%20search&sortby=MP&page=1&bibsperpage=10";
            Assert.AreEqual(expectedUri, handler.LastRequest.RequestUri.AbsoluteUri);
            AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
        }

        [TestMethod]
        public async Task BibBooleanSearchAsync_ThroughInterface_RequestShapeIsStable()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            IPapiClient client = CreateClient(handler);

            var response = await client.BibBooleanSearchAsync("TI=Harry Potter", branchId: 8, page: 3, pageSize: 20, sortBy: SearchSortOptions.AU, TestContext.CancellationToken);

            Assert.IsNotNull(response);
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest!.Method);
            Assert.Contains("/public/v1/1033/100/8/search/bibs/boolean", handler.LastRequest.RequestUri!.AbsolutePath);
            var expectedUri = "https://example.test/PAPIService/REST/public/v1/1033/100/8/search/bibs/boolean?q=TI%3DHarry%20Potter&sortby=AU&page=3&bibsperpage=20";
            Assert.AreEqual(expectedUri, handler.LastRequest.RequestUri.AbsoluteUri);
            var query = ParseQuery(handler.LastRequest.RequestUri.Query);
            Assert.AreEqual("TI=Harry Potter", query["q"]);
            Assert.AreEqual("AU", query["sortby"]);
            Assert.AreEqual("3", query["page"]);
            Assert.AreEqual("20", query["bibsperpage"]);
            Assert.IsNull(handler.LastRequest.Content);
            Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
            AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
        }

        [TestMethod]
        public async Task BibBooleanSearchAsync_ThroughInterfaceWithoutBranchId_UsesOrganizationId()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);
            client.OrganizationId = 42;

            var response = await client.BibBooleanSearchAsync("TI=Default Branch", cancellationToken: TestContext.CancellationToken);

            Assert.IsNotNull(response);
            Assert.IsNotNull(handler.LastRequest);
            Assert.Contains("/public/v1/1033/100/42/search/bibs/boolean", handler.LastRequest!.RequestUri!.AbsolutePath);
            var expectedUri = "https://example.test/PAPIService/REST/public/v1/1033/100/42/search/bibs/boolean?q=TI%3DDefault%20Branch&sortby=MP&page=1&bibsperpage=10";
            Assert.AreEqual(expectedUri, handler.LastRequest.RequestUri.AbsoluteUri);
            AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
        }

        [TestMethod]
        public async Task BibSearchAsync_DefaultLimit_OmitsLimitAndHashesSentUri()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);
            var options = new BibSearchOptions
            {
                Branch = 1,
                SearchType = BibSearchTypes.keyword,
                Qualifier = SearchQualifiers.KW,
                Term = "harry potter & stone",
                SortOption = SearchSortOptions.MP,
                Page = 2,
                PageSize = 15
            };

            var response = await client.BibSearchAsync(options, TestContext.CancellationToken);

            Assert.IsNotNull(response);
            Assert.IsNotNull(handler.LastRequest);
            var expectedUri = "https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?q=harry%20potter%20%26%20stone&sortby=MP&page=2&bibsperpage=15";
            Assert.AreEqual(expectedUri, handler.LastRequest!.RequestUri!.AbsoluteUri);
            var query = ParseQuery(handler.LastRequest.RequestUri.Query);
            Assert.IsFalse(query.ContainsKey("limit"));
            Assert.IsFalse(query.ContainsKey("sort"));
            Assert.IsFalse(query.ContainsKey("notran"));
            AssertAuthorizationHashesSentUri(handler.LastRequest, string.Empty);
        }

        [TestMethod]
        public async Task BibSearchAsync_MapsNativeMaterialTypeAndSuppressesTransactionWhenRequested()
        {
            var handler = new CapturingHttpMessageHandler(CreateJson(new
            {
                PAPIErrorCode = 1,
                TotalRecordsFound = 1,
                BibSearchRows = new[] { new { ControlNumber = 9001, PrimaryTypeOfMaterial = 36 } }
            }));
            var client = CreateClient(handler);
            var response = await client.BibSearchAsync(new BibSearchOptions
            {
                Term = "test title",
                Branch = 43,
                Qualifier = SearchQualifiers.TI,
                SortOption = SearchSortOptions.PDTI,
                NoTransaction = true
            }, TestContext.CancellationToken);

            Assert.IsNotNull(response.Data);
            Assert.HasCount(1, response.Data.BibSearchRows);
            Assert.AreEqual(9001, response.Data.BibSearchRows[0].ControlNumber);
            Assert.AreEqual(36, response.Data.BibSearchRows[0].PrimaryTypeOfMaterial);
            var uri = GetLastRequestUri(handler);
            Assert.AreEqual("/PAPIService/REST/public/v1/1033/100/43/search/bibs/keyword/TI", uri.AbsolutePath);
            var query = ParseQuery(uri.Query);
            Assert.AreEqual("PDTI", query["sortby"]);
            Assert.AreEqual("1", query["notran"]);
            Assert.IsFalse(query.ContainsKey("sort"));
            AssertAuthorizationHashesSentUri(handler.LastRequest!, string.Empty);
        }

        [TestMethod]
        public async Task BibSearchAsync_UsesDocumentedBooleanUpcAccessPoint()
        {
            var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
            var client = CreateClient(handler);
            await client.BibSearchAsync(new BibSearchOptions
            {
                Term = "UPC=\"733961758009\"",
                Branch = 77,
                SearchType = BibSearchTypes.boolean,
                SortOption = SearchSortOptions.TITOM
            }, TestContext.CancellationToken);

            var uri = GetLastRequestUri(handler);
            Assert.AreEqual("/PAPIService/REST/public/v1/1033/100/77/search/bibs/boolean", uri.AbsolutePath);
            var query = ParseQuery(uri.Query);
            Assert.AreEqual("UPC=\"733961758009\"", query["q"]);
            Assert.AreEqual("TITOM", query["sortby"]);
            Assert.IsFalse(query.ContainsKey("notran"));
            AssertAuthorizationHashesSentUri(handler.LastRequest!, string.Empty);
        }
    }
}
