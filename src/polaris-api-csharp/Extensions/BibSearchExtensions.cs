namespace Clc.Polaris.Api
{
    public static class BibSearchExtensions
    {
        public static Task<IRestResponse<BibSearchResult>> BibKeywordSearchAsync(
            this IPapiClient client,
            string keyword,
            int? branchId = null,
            int page = 1,
            int pageSize = 10,
            SearchSortOptions sortBy = SearchSortOptions.MP,
            CancellationToken cancellationToken = default)
        {
            return client.BibSearchAsync(
                new BibSearchOptions
                {
                    Term = keyword,
                    Branch = branchId ?? client.OrganizationId,
                    Page = page,
                    PageSize = pageSize,
                    SortOption = sortBy
                },
                cancellationToken);
        }

        public static Task<IRestResponse<BibSearchResult>> BibBooleanSearchAsync(
            this IPapiClient client,
            string ccl,
            int? branchId = null,
            int page = 1,
            int pageSize = 10,
            SearchSortOptions sortBy = SearchSortOptions.MP,
            CancellationToken cancellationToken = default)
        {
            return client.BibSearchAsync(
                new BibSearchOptions
                {
                    SearchType = BibSearchTypes.boolean,
                    Term = ccl,
                    Branch = branchId ?? client.OrganizationId,
                    Page = page,
                    PageSize = pageSize,
                    SortOption = sortBy
                },
                cancellationToken);
        }
    }
}
