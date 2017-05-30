using Newtonsoft.Json;

namespace Phaxio.Entities.V2
{
    /// <summary>
    ///  This represents a Phaxio account status in the V2 client
    /// </summary>
    public class AccountStatus
    {
        [JsonProperty(PropertyName = "balance")]
        public int Balance { get; set; }

        [JsonProperty(PropertyName = "faxes_today")]
        public FaxCount FaxesToday { get; set; }

        [JsonProperty(PropertyName = "faxes_this_month")]
        public FaxCount FaxesThisMonth { get; set; }
    }
}
