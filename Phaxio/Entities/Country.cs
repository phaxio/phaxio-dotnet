using Newtonsoft.Json;

namespace Phaxio.Entities
{
    public class Country
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "alpha2")]
        public string Alpha2 { get; set; }

        [JsonProperty(PropertyName = "country_code")]
        public string CountryCode { get; set; }

        [JsonProperty(PropertyName = "price_per_page")]
        public int PricePerPage { get; set; }

        [JsonProperty(PropertyName = "send_support")]
        public string SendSupport { get; set; }

        [JsonProperty(PropertyName = "receive_support")]
        public string ReceiveSupport { get; set; }
    }
}
