using Newtonsoft.Json;
using System;

namespace Phaxio.Entities
{
    public class PhoneNumber
    {
        [JsonProperty(PropertyName = "number")]
        public string Number { get; set; }

        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "cost")]
        public int Cost { get; set; }

        [JsonProperty(PropertyName = "last_billed_at")]
        public DateTime LastBilled { get; set; }

        [JsonProperty(PropertyName = "provisioned_at")]
        public DateTime Provisioned { get; set; }
    }
}
