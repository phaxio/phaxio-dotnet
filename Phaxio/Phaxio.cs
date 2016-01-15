using Newtonsoft.Json;
using Phaxio.Entities.Internal;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phaxio
{
    public class Phaxio
    {
        private const string phaxioApiEndpoint = "https://api.phaxio.com/v1/";
        public const string KeyName = "api_key";
        public const string SecretName = "api_secret";
        private readonly string key;
        private readonly string secret;
        private IRestClient client;

        public Phaxio (string key, string secret)
            : this(key, secret, new RestClient())
        {

        }

        public Phaxio (string key, string secret, IRestClient restClient)
        {
            this.key = key;
            this.secret = secret;

            // Initialize the rest client
            client = restClient;
            client.BaseUrl = new Uri(phaxioApiEndpoint);
        }

        /// <summary>
        ///  Gets the account for this Phaxio instance.
        /// </summary>
        public Account GetAccountStatus ()
        {
            return performRequest<Account>("accountStatus", Method.GET);
        }

        private T performRequest<T> (string resource, Method method)
        {
            var request = new RestRequest();

            request.AddParameter(KeyName, key);
            request.AddParameter(SecretName, secret);

            request.Method = Method.GET;
            request.Resource = resource;

            var response = client.Execute<Response<T>>(request);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response.  Check inner details for more info.";
                var phaxioException = new ApplicationException(message, response.ErrorException);
                throw phaxioException;
            }

            if (!response.Data.Success)
            {
                throw new ApplicationException(response.Data.Message);
            }

            return response.Data.Data;
        }
    }
}
