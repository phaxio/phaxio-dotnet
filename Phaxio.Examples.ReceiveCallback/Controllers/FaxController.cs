using Newtonsoft.Json;
using Phaxio.Examples.ReceiveCallback.Models;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Phaxio.Examples.ReceiveCallback.Controllers
{
    public class FaxController : ApiController
    {
        public HttpResponseMessage Get(string key)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(key, FileMode.Open);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            return result;
        }

        public Task<HttpResponseMessage> Post()
        {
            // Check to make sure we're getting the expected format
            // Phaxio will send the callback as a multipart
            HttpRequestMessage request = this.Request;
            if (!request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            // We'll need a place to store the fax uploads
            // In this case, we'll store it in /App_Data/uploads/
            string root = HttpContext.Current.Server.MapPath("~/App_Data/uploads");
            var provider = new MultipartFormDataStreamProvider(root);

            // Now we're going to request that the provider process
            // the request and when it's finished, call back the
            // lambda below
            var task = request.Content.ReadAsMultipartAsync(provider).
                ContinueWith<HttpResponseMessage>(o =>
                {
                    // Right here, the provider has read all the data
                    // We get the file that Phaxio sent us
                    var file = provider.FileData.First();

                    // Here we get a new Fax object from the key/values
                    // that Phaxio passed us
                    var receipt = new FaxReceipt() {
                        Fax = JsonConvert.DeserializeObject(provider.FormData["fax"])
                    };

                    // Here we'll get the name of the file so we can
                    // reference it later
                    receipt.Key = file.LocalFileName;

                    // We're storing the fax in a memory cache
                    ObjectCache cache = MemoryCache.Default;
                    var faxList = cache["Callbacks"] as List<FaxReceipt>;
                    faxList.Add(receipt);

                    // Respond to Phaxio's servers
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent("Callback received.")
                    };
                }
            );

            return task;
        }
    }
}
