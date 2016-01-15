using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phaxio
{
    /// <summary>
    ///  This represents a Phaxio account
    /// </summary>
    public class Account
    {
        [DeserializeAs(Name = "faxes_sent_this_month")]
        public int FaxesSentThisMonth { get; set; }

        [DeserializeAs(Name = "faxes_sent_today")]
        public int FaxesSentToday { get; set; }

        [DeserializeAs(Name = "balance")]
        public int Balance { get; set; }
    }
}
