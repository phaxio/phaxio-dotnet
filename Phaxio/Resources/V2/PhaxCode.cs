using Newtonsoft.Json;
using Phaxio.ThinRestClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Phaxio.Resources.V2
{
    public class PhaxCode
    {
        public PhaxioContext PhaxioClient { get; set; }

        [JsonProperty(PropertyName = "identifier")]
        public string Identifier { get; set; }

        [JsonProperty(PropertyName = "metadata")]
        public string Metadata { get; set; }

        /// <summary>
        /// Returns a byte array representing PNG of the PhaxCode.
        /// </summary>
        public byte[] Png
        {
            get
            {
                var resource = "phax_code";

                if (Identifier != null)
                {
                    resource += "s/" + WebUtility.UrlEncode(Identifier);
                }

                return PhaxioClient.download(resource + ".png", Method.GET, req => { });
            }
        }
    }
}
