using Newtonsoft.Json;

namespace Phaxio.Entities
{
    public class Pricing
    {
        [JsonProperty(PropertyName = "price_per_page")]
        public int PricePerPage {get;set;}
    }
}
