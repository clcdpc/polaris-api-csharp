using System.IO;
using System.Net.Http;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<ILLRequestResult>> ILLRequestPostAsync(ILLRequestCreateData requestData, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(requestData);
            Require.Positive(requestData.PatronID);
            Require.Argument(requestData.Title);
            Require.NonNegative(requestData.PickupOrgID);
            Require.PositiveIfProvided(requestData.WorkstationID);
            Require.PositiveIfProvided(requestData.UserID);
            Require.PositiveIfProvided(requestData.HoldPickupAreaID);

            var url = $"/public/v1/1033/100/{OrganizationId}/illrequest";
            var request = PapiRestRequest.Post(url);
            request.Content = new StringContent(SerializeXml(requestData), Encoding.UTF8, "application/xml");
            return await ExecutePapiAsync<ILLRequestResult>(request, cancellationToken).ConfigureAwait(false);
        }

        private static string SerializeXml<T>(T value)
        {
            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true
            };

            using var writer = new StringWriter();
            using var xmlWriter = XmlWriter.Create(writer, settings);
            new XmlSerializer(typeof(T)).Serialize(xmlWriter, value);
            return writer.ToString();
        }
    }
}
