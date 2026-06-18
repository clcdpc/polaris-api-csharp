namespace Clc.Polaris.Api.Models
{
    public class PatronAuthenticationResult : PapiResponseCommon
    {
        public string? AccessToken { get; set; }
        public string? AccessSecret { get; set; }
        public int PatronID { get; set; }

        public override string ToString() => PatronID.ToString();
    }
}
