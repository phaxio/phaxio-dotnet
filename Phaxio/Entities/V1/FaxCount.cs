using Newtonsoft.Json;

namespace Phaxio.Entities
{
    public class FaxCount
    {
        [JsonProperty(PropertyName = "sent")]
        public int Sent { get; set; }

        [JsonProperty(PropertyName = "received")]
        public int Received { get; set; }
    }
}
