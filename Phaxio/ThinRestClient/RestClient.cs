using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Phaxio.ThinRestClient
{
    class RestClient : IRestClient
    {
        public Uri BaseUrl { get; set; }

        public RestClient()
        {
        }

        public IRestResponse Execute(IRestRequest request)
        {
            HttpResponseMessage response = performRequest(request);

            return new RestResponse(response);
        }

        public IRestResponse<T> Execute<T>(IRestRequest request)
        {
            HttpResponseMessage response = performRequest(request);
            
            return new RestResponse<T>(response);
        }

        private HttpResponseMessage performRequest(IRestRequest request)
        {
            var client = new HttpClient();

            client.BaseAddress = BaseUrl;

            HttpResponseMessage response;

            if (request.Method == Method.GET)
            {
                response = performGet(client, request);
            }
            else if (request.Method == Method.POST)
            {
                response = performPost(client, request);
            }
            else
            {
                throw new NotImplementedException();
            }

            return response;
        }

        private HttpResponseMessage performGet(HttpClient client, IRestRequest request)
        {
            return client.GetAsync(request.Resource + getParametersAsQuery(request)).Result;
        }

        private HttpResponseMessage performPost(HttpClient client, IRestRequest request)
        {
            HttpContent content;

            if (request.Files.Any())
            {
                var boundary = "--------------------------------" + DateTime.Now.ToString(CultureInfo.InvariantCulture);
                var multipartContent = new MultipartFormDataContent(boundary);

                foreach (var param in request.Parameters)
                {
                    multipartContent.Add(new StringContent(param.Value.ToString()), param.Name);
                }
                
                foreach (var file in request.Files)
                {
                    multipartContent.Add(new StreamContent(new MemoryStream(file.Bytes)), file.Name, file.FileName);
                }

                content = multipartContent;
            }
            else
            {
                var parameters = request.Parameters.ToDictionary(x => x.Name, x => x.Value.ToString());
                content = new FormUrlEncodedContent(parameters);
            }
            
            return client.PostAsync(request.Resource, content).Result;
        }

        private string getParametersAsQuery(IRestRequest request)
        {
            if (!request.Parameters.Any())
            {
                return "";
            }
            
            var sb = new StringBuilder("?");

            var separator = "";
            foreach (var param in request.Parameters.Where(param => param.Value != null))
            {
                var encodedName = WebUtility.UrlEncode(param.Name);
                var encodedValue = WebUtility.UrlEncode(param.Value.ToString());
                sb.AppendFormat("{0}{1}={2}", separator, encodedName, encodedValue);

                separator = "&";
            }

            return sb.ToString();
        }
    }
}
