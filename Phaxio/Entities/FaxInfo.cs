using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phaxio.Entities
{
    public class FaxInfo
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "direction")]
        public string Direction { get; set; }

        [JsonProperty(PropertyName = "num_pages")]
        public int PageCount { get; set; }

        [JsonProperty(PropertyName = "cost")]
        public int CostInCents { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "is_test")]
        public bool IsTest { get; set; }

        [JsonProperty(PropertyName = "created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty(PropertyName = "from_number")]
        public string FromNumber { get; set; }

        [JsonProperty(PropertyName = "to_number")]
        public string ToNumber { get; set; }

        [JsonProperty(PropertyName = "recipients")]
        public IEnumerable<Recipient> Recipients { get; set; }

        [JsonProperty(PropertyName = "tags")]
        public Dictionary<string, string> Tags { get; set; }

        [JsonProperty(PropertyName = "error_type")]
        public string ErrorType { get; set; }

        [JsonProperty(PropertyName = "error_id")]
        public int? ErrorId { get; set; }

        [JsonProperty(PropertyName = "error_message")]
        public string ErrorMessage { get; set; }

        [JsonProperty(PropertyName = "completed_at")]
        public DateTime CompletedAt { get; set; }
    }
}
