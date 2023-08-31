using Clc.Rest;
using Clc.Polaris.Api.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<BibSearchResult> BibSearch(BibSearchOptions options)
        {
            var url = $"/public/v1/1033/100/{options.Branch}/search/bibs/{options.SearchType}";
            if (options.SearchType == BibSearchTypes.keyword) { url += $"/{options.Qualifier}"; }
            url += $"?q={WebUtility.UrlEncode(options.Term)}&sort={options.SortOption}&page={options.Page}&bibsperpage={options.PageSize}&limit={options.Limit}";

            var request = new PapiRestRequest(url) { BlockStaffOverride = true };

            return Execute<BibSearchResult>(request);
        }

        public IRestResponse<BibSearchResult> BibKeywordSearch(string keyword, int? branchId = null, int page = 1, int pageSize = 10, SearchSortOptions sortBy = SearchSortOptions.MP)
        {
            return BibSearch(new BibSearchOptions { Term = keyword, Branch = branchId ?? OrganizationId, Page = page, PageSize = pageSize, SortOption = sortBy });
        }

        public IRestResponse<BibSearchResult> BibBooleanSearch(string ccl, int? branchId = null, int page = 1, int pageSize = 10, SearchSortOptions sortBy = SearchSortOptions.MP)
        {
            return BibSearch(new BibSearchOptions { SearchType = BibSearchTypes.boolean, Term = ccl, Branch = branchId ?? OrganizationId, Page = page, PageSize = pageSize, SortOption = sortBy });
        }
    }
}
