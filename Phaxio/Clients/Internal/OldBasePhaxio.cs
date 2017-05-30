using Phaxio.Errors.V2;
using Phaxio.Entities.Internal;
using Phaxio.ThinRestClient;
using Phaxio.ThinRestClient.Helpers;
using Phaxio.Errors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;

namespace Phaxio
{
    public class OldBasePhaxio
    {
        protected readonly string key;
        protected readonly string secret;
        protected IRestClient client;

        protected OldBasePhaxio(string key, string secret) : this(key, secret, new RestClient()) { }

        protected OldBasePhaxio(string key, string secret, IRestClient restClient)
        {
            this.key = key;
            this.secret = secret;

            // Initialize the rest client
            client = restClient;
        }

        internal byte[] download(string resource, Method method, Action<IRestRequest> requestModifier)
        {
            var request = prepareRequest(resource, method, true, requestModifier);

            var response = client.Execute(request);

            checkException(response);

            return response.RawBytes;
        }

        internal Response<T> request<T>(string resource, Method method)
        {
            return request<T>(resource, method, true, r => { });
        }

        internal Response<List<T>> requestList<T>(string resource, Method method, bool auth, Action<IRestRequest> requestModifier)
        {
            var response = request<List<T>>(resource, method, auth, requestModifier);

            foreach(var item in response.Data)
            {
                addPhaxioClient(item);
            }

            return response;
        }

        internal Response<T> request<T>(string resource, Method method, bool auth, Action<IRestRequest> requestModifier)
        {
            var request = prepareRequest(resource, method, auth, requestModifier);

            var response = client.Execute<Response<T>>(request);

            addPhaxioClient(response.Data.Data);

            checkException(response);

            return response.Data;
        }

        internal IEnumerable<T> pagedRequest<T>(string resource, Method method, bool auth, Action<IRestRequest> requestModifier)
        {
            var response = requestList<T> (resource, method, auth, requestModifier);

            var totalPages = Math.Ceiling((decimal)response.PagingInfo.Total / response.PagingInfo.PerPage);

            var dataPage = response.Data;

            // Outer loop is by page
            for (int i = 0; i < totalPages; i++)
            {
                foreach (var item in dataPage)
                {
                    yield return item;
                }

                if (i < (totalPages - 1))
                {
                    Action<IRestRequest> addParametersForNextPage = req =>
                    {
                        requestModifier.Invoke(req);

                        req.AddParameter("page", i + 2);
                    };

                    dataPage = requestList<T>(resource, method, auth, addParametersForNextPage).Data;
                }
            }
        }

        private RestRequest prepareRequest(string resource, Method method, bool auth, Action<IRestRequest> requestModifier)
        {
            var request = new RestRequest();

            if (auth)
            {
                request.AddParameter(PhaxioConstants.KEY_NAME, key);
                request.AddParameter(PhaxioConstants.SECRET_NAME, secret);
            }

            // Run any custom modifications
            requestModifier(request);

            request.Method = method;
            request.Resource = resource;

            return request;
        }

        private void checkException(IRestResponse response)
        {
            handleNetworkException(response);

            if (response.ContentType == "application/json")
            {
                var json = new JsonDeserializer();

                var phaxioException = json.Deserialize<Response<object>>(response);

                handlePhaxioException(response, phaxioException);
            }
        }

        private void checkException<T>(IRestResponse<Response<T>> response)
        {
            handleNetworkException(response);
            handlePhaxioException(response, response.Data);
        }

        private void handleNetworkException(IRestResponse response)
        {
            if (response.ErrorException != null)
            {
                throw new ApiConnectionException(response.ErrorException.Message);
            }
        }

        private void handlePhaxioException<T>(IRestResponse restResponse, Response<T> apiResponse)
        {
            if (restResponse.StatusCode == (HttpStatusCode)429) // Rate limit
            {
                throw new RateLimitException(apiResponse.Message);
            }
            else if (restResponse.StatusCode == (HttpStatusCode)422) // Invalid entity
            {
                throw new InvalidRequestException(apiResponse.Message);
            }
            else if (restResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new AuthenticationException(apiResponse.Message);
            }
            else if (restResponse.StatusCode == HttpStatusCode.NotFound)
            {
                throw new NotFoundException(apiResponse.Message);
            }
            else if ((int)restResponse.StatusCode >= 500 && (int)restResponse.StatusCode < 600) // 500 errors
            {
                throw new ServiceException(apiResponse.Message);
            }
        }

        private void addPhaxioClient<T>(T item)
        {
            PropertyInfo property = typeof(T).GetProperty("PhaxioClient");

            if (property != null)
            {
                property.SetValue(item, this, null);
            }
        }
    }
}
