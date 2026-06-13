using System;

namespace Clc.Polaris.Api.Models
{
    public class PatronAuthenticationToken
    {
        public string? Barcode { get; set; }
        public string? Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
