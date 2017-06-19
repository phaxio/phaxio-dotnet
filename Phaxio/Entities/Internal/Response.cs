using Newtonsoft.Json;
using Phaxio.Entities;
using System.Collections.Generic;

namespace Phaxio
{
    public class Response<T>
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "data")]
        public T Data { get; set; }

        [JsonProperty(PropertyName = "paging")]
        public PagingInfo PagingInfo { get; set; }

        public Result ToResult()
        {
            return new Result { Success = Success, Message = Message };
        }
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
