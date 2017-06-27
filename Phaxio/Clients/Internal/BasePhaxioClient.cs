using Phaxio.Entities.Internal;
using Phaxio.ThinRestClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phaxio.Clients.Internal
{
    public abstract class BasePhaxioClient
    {
        protected readonly string key;
        protected readonly string secret;
        protected IRestClient client;

        public BasePhaxioClient(string key, string secret)
            : this(key, secret, new RestClient())
        {

        }

        public BasePhaxioClient(string key, string secret, IRestClient restClient)
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

        internal Response<T> request<T>(string resource, Method method, bool auth, Action<IRestRequest> requestModifier)
        {
            var request = prepareRequest(resource, method, auth, requestModifier);

            var response = client.Execute<Response<T>>(request);

            checkException(response);

            modifyDataItem(response.Data.Data);

            return response.Data;
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

        protected abstract void checkException(IRestResponse response);
        protected abstract void checkException<T>(IRestResponse<Response<T>> response);
        protected abstract void modifyDataItem<T>(T item);
    }
}
