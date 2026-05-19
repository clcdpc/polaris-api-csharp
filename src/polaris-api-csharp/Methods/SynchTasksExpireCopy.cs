using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net.Http;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<SynchTasksExpireCopyResult> SynchTasksExpireCopy(SynchTasksExpireCopyData data, int? userId = null, int? workstationId = null)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/synch/tasks/expirecopy?userid={userId ?? UserId}&wsid={workstationId ?? WorkstationId}";
            var request = new PapiRestRequest(HttpMethod.Post, url) { Body = data };
            return Execute<SynchTasksExpireCopyResult>(request);
        }
    }
}
