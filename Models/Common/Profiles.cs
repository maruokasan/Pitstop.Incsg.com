namespace Pitstop.Models.Common
{
    public class ProfilesOption
    {
        public const string Profiles = "Profiles";
        public string Server_Name { get; set; } = string.Empty;
        public string Database_Name { get; set; } = string.Empty;
        public string License_UserName { get; set; } = string.Empty;
        public string License_Password { get; set; } = string.Empty;
        public string Database_UserName { get; set; } = string.Empty;
        public string Database_Password { get; set; } = string.Empty;
        public string Is_Company_Connected { get; set; } = string.Empty;

    }
}
