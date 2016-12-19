using Newtonsoft.Json;

namespace Phaxio.Entities
{
    /// <summary>
    ///  This represents a Phaxio account
    /// </summary>
    public class Account
    {
        [JsonProperty(PropertyName = "faxes_sent_this_month")]
        public int FaxesSentThisMonth { get; set; }

        [JsonProperty(PropertyName = "faxes_sent_today")]
        public int FaxesSentToday { get; set; }

        [JsonProperty(PropertyName = "balance")]
        public int Balance { get; set; }
    }
}
