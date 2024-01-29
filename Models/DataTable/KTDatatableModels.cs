using Newtonsoft.Json;

namespace Pitstop.Models.DataTable
{
    public class KTResult<T>
    {
        public DataTablePagination meta { get; set; }

        [JsonProperty("data")]
        public T data { get; set; }
    }

    public class KTParameters
    {
        public DataTablePagination pagination { get; set; }
        public DataTableQuery query { get; set; }
        public DataTableSort sort { get; set; }
    }
    public class DataTableQuery
    {
        public string generalSearch { get; set; }
        public string status { get; set; }
        public bool? isPrint { get; set; }
    }
    public class DataTablePagination
    {
        public int page { get; set; }
        public int pages { get; set; }
        public int perpage { get; set; }
        public int total { get; set; }
    }

    public class DataTableSort
    {
        public string field { get; set; }
        public string sort { get; set; }
    }
}
