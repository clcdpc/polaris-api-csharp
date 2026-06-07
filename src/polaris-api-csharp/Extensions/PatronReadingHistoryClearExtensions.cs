using Clc.Polaris.Api.Models;
using Clc.Rest;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public static class PatronReadingHistoryClearExtensions
    {
        public static Task<IRestResponse<PapiResponseCommon>> PatronReadingHistoryClearAsync(
            this IPapiClient client,
            string barcode,
            IEnumerable<int> ids,
            CancellationToken cancellationToken = default)
        {
            return client.PatronReadingHistoryClearAsync(barcode, null, ids, cancellationToken);
        }
    }
}
