namespace Pitstop.Models.Common
{
    public class AppKeysOption
    {
        public const string AppKeys = "AppKeys";
        public string KeyRingPath { get; set; } = string.Empty;
        public string DefaultLoginUrl { get; set; } = string.Empty;
        public string AttachmentUrl { get; set; } = string.Empty;
        public string AttachmentKey { get; set; } = string.Empty;
    }
}
