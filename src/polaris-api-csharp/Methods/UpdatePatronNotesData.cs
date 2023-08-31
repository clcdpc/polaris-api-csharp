using Clc.Polaris.Api.Models;
using Clc.Polaris.Models;
using Clc.Rest;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<PapiResponseCommon> UpdatePatronNotesData(string barcode, string nonBlockingNote = null, string blockingNote = null, UpdateNoteMode updateMode = UpdateNoteMode.Prepend, int? workstationId = null)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/patron/{barcode}/notes?wsid={workstationId ?? WorkstationId}";
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

            return Post<PapiResponseCommon>(url, body: body);
        }
    }
}
