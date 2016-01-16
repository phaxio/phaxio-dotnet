using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phaxio.Entities
{
    public class PhoneNumber
    {
        [DeserializeAs(Name = "number")]
        public string Number { get; set; }

        [DeserializeAs(Name = "city")]
        public string City { get; set; }

        [DeserializeAs(Name = "state")]
        public string State { get; set; }

        [DeserializeAs(Name = "cost")]
        public int Cost { get; set; }

        [DeserializeAs(Name = "last_billed_at")]
        public DateTime LastBilled { get; set; }

        [DeserializeAs(Name = "provisioned_at")]
        public DateTime Provisioned { get; set; }
    }
}
