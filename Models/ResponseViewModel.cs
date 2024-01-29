namespace Pitstop.Models
{
    public class ResponseViewModel<T> where T : class
    {
        public ResponseViewModel()
        {

        }

        public ResponseViewModel(T data, string message = null)
        {
            Data = data;
            Message = message;
        }

        public T Data { get; set; }
        public string Message { get; set; }
    }
}
