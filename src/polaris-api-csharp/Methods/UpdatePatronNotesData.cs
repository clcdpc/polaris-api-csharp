using Clc.Polaris.Api.Models;
using Clc.Polaris.Models;
using Clc.Rest;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<PapiResponseCommon>> UpdatePatronNotesDataAsync(string barcode, string nonBlockingNote = null, string blockingNote = null, UpdateNoteMode updateMode = UpdateNoteMode.Prepend, int? workstationId = null, CancellationToken cancellationToken = default)
        {
            var url = $"/protected/v1/1033/100/1/{ProtectedToken.Placeholder}/patron/{WebUtility.UrlEncode(barcode)}/notes";
            var body = new UpdatePatronNotesData();

            if (!string.IsNullOrWhiteSpace(nonBlockingNote))
            {
                if (updateMode == UpdateNoteMode.Prepend && !nonBlockingNote.EndsWith("\r\n")) { nonBlockingNote = $"{nonBlockingNote}\r\n"; }
                if (updateMode == UpdateNoteMode.Append && !nonBlockingNote.StartsWith("\r\n")) { nonBlockingNote = $"\r\n{nonBlockingNote}"; }
                body.NonBlockingNote = nonBlockingNote;
                body.NonBlockingNoteMode = (int)updateMode;
            }

            if (!string.IsNullOrWhiteSpace(blockingNote))
            {
                if (updateMode == UpdateNoteMode.Prepend && !blockingNote.EndsWith("\r\n")) { blockingNote = $"{blockingNote}\r\n"; }
                if (updateMode == UpdateNoteMode.Append && !blockingNote.StartsWith("\r\n")) { blockingNote = $"\r\n{blockingNote}"; }
                body.BlockingNote = blockingNote;
                body.BlockingNoteMode = (int)updateMode;
            }

            var request = PapiRestRequest.Post(url, body: body);
            request.QueryParameters.Add("wsid", workstationId ?? WorkstationId);
            return await ExecutePapiAsync<PapiResponseCommon>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
