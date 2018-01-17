using Newtonsoft.Json;
using Phaxio.ThinRestClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phaxio.Resources.V2
{
    public class PhoneNumber
    {
        public PhaxioClient PhaxioClient { private get; set; }

        [JsonProperty(PropertyName = "phone_number")]
        public string Number { get; set; }

        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }

        [JsonProperty(PropertyName = "cost")]
        public int Cost { get; set; }

        [JsonProperty(PropertyName = "last_billed_at")]
        public DateTime LastBilledAt { get; set; }

        [JsonProperty(PropertyName = "provisioned_at")]
        public DateTime ProvisionedAt { get; set; }

        [JsonProperty(PropertyName = "callback_url")]
        public string CallbackUrl { get; set; }

        /// <summary>
        ///  Releases a number
        /// </summary>
        public void Release()
        {
            PhaxioClient.request<Object>("phone_numbers/" + Number, Method.DELETE);
        }
    }
}
