using Phaxio.Examples.ReceiveCallback.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
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