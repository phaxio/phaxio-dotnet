using System.IO;
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

            phaxio.Fax.TestRecieveCallback(new FileInfo(filepath));

            return true;
        }
    }
}
