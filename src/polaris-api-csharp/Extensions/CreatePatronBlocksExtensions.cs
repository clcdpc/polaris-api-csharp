using System.Globalization;

namespace Clc.Polaris.Api
{
    public static class CreatePatronBlocksExtensions
    {
        public static Task<IRestResponse<CreatePatronBlocksResult>> CreatePatronFreeTextBlockAsync(
            this IPapiClient client,
            string barcode,
            string blockText,
            int? userId = null,
            int? workstationId = null,
            CancellationToken cancellationToken = default)
        {
            return client.CreatePatronBlocksAsync(barcode, BlockType.FreeText, blockText, userId, workstationId, cancellationToken);
        }

        public static Task<IRestResponse<CreatePatronBlocksResult>> CreatePatronLibraryAssignedBlockAsync(
            this IPapiClient client,
            string barcode,
            int blockId,
            int? userId = null,
            int? workstationId = null,
            CancellationToken cancellationToken = default)
        {
            Require.Positive(blockId);

            return client.CreatePatronBlocksAsync(barcode, BlockType.LibraryAssigned, blockId.ToString(CultureInfo.InvariantCulture), userId, workstationId, cancellationToken);
        }

        public static Task<IRestResponse<CreatePatronBlocksResult>> CreatePatronSystemBlockAsync(
            this IPapiClient client,
            string barcode,
            SystemBlocks systemBlock,
            int? userId = null,
            int? workstationId = null,
            CancellationToken cancellationToken = default)
        {
            return client.CreatePatronBlocksAsync(barcode, BlockType.System, ((int)systemBlock).ToString(CultureInfo.InvariantCulture), userId, workstationId, cancellationToken);
        }
    }
}
