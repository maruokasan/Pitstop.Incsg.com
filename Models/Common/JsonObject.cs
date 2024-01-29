namespace Pitstop.Models.Common
{
    public class JsonObject<T>
    {
        public T? Data { get; set; }
        public string? Message { get; set; }
        public string? InfoMessage { get; set; }
    }
}
