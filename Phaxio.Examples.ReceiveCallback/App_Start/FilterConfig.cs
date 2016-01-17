using System.Web;
using System.Web.Mvc;

namespace Phaxio.Examples.ReceiveCallback
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
