namespace Pitstop.Models
{
    public class EmailConfiguration
    {
        public string SMTPServer { get; set; }
        public int Port { get; set; }
        public string FromUserNameEmail { get; set; }
        public string FromUserNamePassword { get; set; }
        public string AppsUrl { get; set; }
        public bool isEnableSSL {get; set;}
    }
}
