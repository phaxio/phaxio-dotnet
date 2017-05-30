using Newtonsoft.Json;
using System;

namespace Phaxio.Entities.Internal.V1
{
    public class Url
    {
        [JsonProperty(PropertyName = "url")]
        public Uri Address { get; set; }
    }
}
