using System.IO;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Phaxio.Examples.ReceiveCallback.Controllers
{
    public class TriggerController : Controller
    {
        [HttpPost]
        public ActionResult Index()
        {
            string key = Request.Form["key"];
            string secret = Request.Form["secret"];
            var phaxio = new PhaxioClient(key, secret);

            var filepath = HostingEnvironment.MapPath("~/App_Data/test.pdf");

            var result = phaxio.Fax.TestRecieveCallback(new FileInfo(filepath));

            if (!result.Success)
            {
                return Content("Trigger did not work, unfortunately: " + result.Message);
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}
