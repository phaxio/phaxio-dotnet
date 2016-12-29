using Phaxio.Examples.ReceiveCallback.Models;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Web.Mvc;

namespace Phaxio.Examples.ReceiveCallback.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ObjectCache cache = MemoryCache.Default;

            var faxList = cache["Callbacks"] as List<FaxReceipt>;

            return View(faxList);
        }
    }
}