using Newtonsoft.Json;
using Phaxio.Examples.ReceiveCallback.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
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
            HttpRequestMessage request = this.Request;
            if (!request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/uploads");
            var provider = new MultipartFormDataStreamProvider(root);

            var task = request.Content.ReadAsMultipartAsync(provider).
                ContinueWith<HttpResponseMessage>(o =>
                {
                    var file = provider.FileData.First();

                    var fax = new Fax
                    {
                        Direction = provider.FormData["direction"],
                        Json = JsonConvert.DeserializeObject(provider.FormData["fax"]),
                        IsTest = provider.FormData["is_test"] == "true" ? true : false,
                        Success = provider.FormData["success"] == "true" ? true : false,
                        Key = file.LocalFileName
                    };

                    ObjectCache cache = MemoryCache.Default;

                    var faxList = cache["Callbacks"] as List<Fax>;

                    faxList.Add(fax);

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
