using Phaxio.Entities.Internal;
using Phaxio.ThinRestClient;
using Phaxio.ThinRestClient.Helpers;
using System;

namespace Phaxio.Clients.Internal
{
    public abstract class OldBasePhaxioClient
    {
        protected readonly string key;
        protected readonly string secret;
        protected IRestClient client;

        public OldBasePhaxioClient(string key, string secret)
            : this(key, secret, new RestClient())
        {

        }

        public OldBasePhaxioClient(string key, string secret, IRestClient restClient)
        {
            this.key = key;
            this.secret = secret;

            // Initialize the rest client
            client = restClient;
        }

        protected byte[] download(string resource, Method method, Action<IRestRequest> requestModifier)
        {
            IRestResponse response = null;

            runRequest(resource, method, true, requestModifier, (request) =>
            {
                response = client.Execute(request);
                return response.ErrorException;
            });

            if (response.ContentType == "application/json")
            {
                var json = new JsonDeserializer();

                var phaxioResponse = json.Deserialize<Response<Object>>(response);

                throw new ApplicationException(phaxioResponse.Message);
            }

            return response.RawBytes;
        }

        protected Response<T> request<T>(string resource, Method method)
        {
            return request<T>(resource, method, true, r => { });
        }

        protected Response<T> request<T>(string resource, Method method, bool auth, Action<IRestRequest> requestModifier)
        {
            IRestResponse<Response<T>> response = null;

            runRequest(resource, method, auth, requestModifier, (request) =>
            {
                response = client.Execute<Response<T>>(request);
                return response.ErrorException;
            });

            // If T is Object, it means that the method will return a
            // bool indicating success or failure.
            if (typeof(T) != typeof(Object) && !response.Data.Success)
            {
                throw new ApplicationException(response.Data.Message);
            }

            return response.Data;
        }

        protected void runRequest(string resource, Method method, bool auth, Action<IRestRequest> requestModifier, Func<IRestRequest, Exception> executor)
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

            var exception = executor(request);

            if (exception != null)
            {
                const string message = "Error retrieving response. Check inner exception.";
                throw new ApplicationException(message, exception);
            }
        }
    }
}
