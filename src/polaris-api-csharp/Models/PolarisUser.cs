namespace Clc.Polaris.Api.Models
{
    /// <summary>
    /// Staff member credentials for protected methods and public method override
    /// </summary>
    public class PolarisUser
    {
        /// <summary>
        /// Domain
        /// </summary>
        public string Domain { get; set; } = string.Empty;

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; } = string.Empty;

        public PolarisUser()
        {

        }

        public PolarisUser(string domain, string username, string password)
        {
            Domain = domain;
            Username = username;
            Password = password;
        }
    }
}
