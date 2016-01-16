using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phaxio.Entities
{
    public class Pricing
    {
        [DeserializeAs(Name = "price_per_page")]
        public int PricePerPage {get;set;}
    }
}
