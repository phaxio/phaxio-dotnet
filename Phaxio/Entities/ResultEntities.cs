using Newtonsoft.Json;
using System.Collections.Generic;

namespace Phaxio.Entities
{
    public class Result
    {
        public bool Success { get; set; }

        public string Message { get; set; }
    }

    public class PagedResult<T> : Result
    {
        public List<T> Data { get; set; }

        public PagingInfo PagingInfo { get; set; }
    }

    public class PagingInfo
    {
        [JsonProperty(PropertyName = "total")]
        public int Total { get; set; }

        [JsonProperty(PropertyName = "per_page")]
        public int PerPage { get; set; }

        [JsonProperty(PropertyName = "page")]
        public int Page { get; set; }
    }
}
