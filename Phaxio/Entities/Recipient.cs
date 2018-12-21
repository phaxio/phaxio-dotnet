using Newtonsoft.Json;
using System;

namespace Phaxio.Entities
{
    public class Recipient
    {
        [JsonProperty(PropertyName = "phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "retry_count")]
        public int RetryCount { get; set; }

        [JsonProperty(PropertyName = "completed_at")]
        public DateTime? CompletedAt { get; set; }

        [JsonProperty(PropertyName = "bitrate")]
        public int Bitrate { get; set; }

        [JsonProperty(PropertyName = "resolution")]
        public int Resolution { get; set; }

        [JsonProperty(PropertyName = "error_type")]
        public string ErrorType { get; set; }

        [JsonProperty(PropertyName = "error_id")]
        public int? ErrorId { get; set; }

        [JsonProperty(PropertyName = "error_message")]
        public string ErrorMessage { get; set; }
    }
}
