using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Phaxio.ThinRestClient.Helpers
{
    static class HttpResponseMessageExtensions
    {
        public static string GetHeader(this HttpResponseMessage message, string headerName)
        {
            HttpHeaders headers = message.Headers;
            IEnumerable<string> values;
            if (headers.TryGetValues(headerName, out values))
            {
                return values.First();
            }
            else
            {
                return null;
            }
        }
    }
}
