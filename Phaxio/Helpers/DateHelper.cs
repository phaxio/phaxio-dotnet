using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phaxio.Helpers
{
    internal static class DateHelper
    {
        internal static string Rfc3339(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd'T'HH:mm:ssK", DateTimeFormatInfo.InvariantInfo);
        }
    }
}
