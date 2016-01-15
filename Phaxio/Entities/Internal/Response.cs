using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phaxio.Entities.Internal
{
    public class Response<T>
    {
        [DeserializeAs(Name = "success")]
        public bool Success { get; set; }

        [DeserializeAs(Name = "message")]
        public string Message { get; set; }

        [DeserializeAs(Name = "data")]
        public T Data { get; set; }
    }
}
