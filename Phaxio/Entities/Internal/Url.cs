using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phaxio.Entities.Internal
{
    public class Url
    {
        [DeserializeAs(Name = "url")]
        public Uri Address { get; set; }
    }
}
