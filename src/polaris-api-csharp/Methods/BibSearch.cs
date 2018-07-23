using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        /// <summary>
        /// Search for bibliographic records
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public PapiResponse<BibSearchResult> BibSearch(BibSearchOptions options)
        {
            var url = $"/PAPIService/REST/public/v1/1033/100/{options.Branch}/search/bibs/{options.SearchType}/{options.Qualifier}?q={PolarisEncode(options.Term)}&sort={options.SortOption}&page={options.Page}&bibsperpage={options.PageSize}";
            if (!string.IsNullOrWhiteSpace(options.Limit)) url += $"&limit={options.Limit}";
            return Execute<BibSearchResult>(HttpMethod.Get, url);
        }

        public PapiResponse<BibSearchResult> BibKeywordSearch(string term, int branchId = 1, int pageSize = 10)
        {
            var options = new BibSearchOptions
            {
                Branch = branchId,
                PageSize = pageSize,
                Term = term
            };

            return BibSearch(options);
        }
    }
}
