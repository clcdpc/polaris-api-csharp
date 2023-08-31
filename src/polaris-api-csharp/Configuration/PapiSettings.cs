using Clc.Polaris.Api.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clc.Polaris.Api.Configuration
{
    public class PapiSettings : IPapiSettings
    {
        public const string SECTION_NAME = "PapiSettings";

        public string AccessId { get; set; }
        public string AccessKey { get; set; }
        public string Hostname { get; set; }
        public PolarisUser PolarisOverrideAccount { get; set; }
    }
    public interface IPapiSettings
    {
        string AccessId { get; set; }
        string AccessKey { get; set; }
        string Hostname { get; set; }
        PolarisUser PolarisOverrideAccount { get; set; }
    }
}
