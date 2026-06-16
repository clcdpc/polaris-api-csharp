
namespace Clc.Polaris.Api.Configuration
{
    public class PapiSettings : IPapiSettings
    {
        public const string SECTION_NAME = "PapiSettings";

        public string AccessId { get; set; } = string.Empty;
        public string AccessKey { get; set; } = string.Empty;
        public string Hostname { get; set; } = string.Empty;
        public int OrganizationId { get; set; } = 1;
        public int UserId { get; set; } = 1;
        public int WorkstationId { get; set; } = 1;
        public PolarisUser? PolarisOverrideAccount { get; set; }
    }
    public interface IPapiSettings
    {
        string AccessId { get; set; }
        string AccessKey { get; set; }
        string Hostname { get; set; }
        int OrganizationId { get; set; }
        int UserId { get; set; }
        int WorkstationId { get; set; }
        PolarisUser? PolarisOverrideAccount { get; set; }
    }
}
