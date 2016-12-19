using Newtonsoft.Json;
using System;

namespace Phaxio.Entities.Internal
{
    public class Url
    {
        [JsonProperty(PropertyName = "url")]
        public Uri Address { get; set; }
    }
}
