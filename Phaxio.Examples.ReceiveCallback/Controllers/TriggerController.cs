using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Hosting;
using System.Web.Http;

namespace Phaxio.Examples.ReceiveCallback.Controllers
{
    public class TriggerController : ApiController
    {
        public bool Get(string key, string secret)
        {
            var phaxio = new PhaxioClient(key, secret);

            var filepath = HostingEnvironment.MapPath("~/App_Data/test.pdf");

            phaxio.TestRecieveCallback(new FileInfo(filepath));

            return true;
        }
    }
}
