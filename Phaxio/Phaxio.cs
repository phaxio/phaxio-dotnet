using Newtonsoft.Json;
using Phaxio.Entities;
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
        /// <returns>An Account object</returns>
        public Account GetAccountStatus ()
        {
            return performRequest<Account>("accountStatus", Method.GET).Data;
        }

        /// <summary>
        ///  Displays a list of area codes available for purchasing Phaxio numbers
        /// </summary>
        /// <param name="tollFree">Whether the number should be tollfree.</param>
        /// <param name="state">A two character state or province abbreviation (e.g. IL or YT).
        /// Will only return area codes available for this state.</param>
        /// <returns>A Dictionary<string, CityState> with area codes for keys and CityStates for values</returns>
        public Dictionary<string, CityState> GetAreaCodes (bool? tollFree = null, string state = null)
        {
            Action<IRestRequest> addParameters = req =>
                {
                    if (tollFree != null)
                    {
                        req.AddParameter("is_toll_free", tollFree);
                    }

                    if (state != null)
                    {
                        req.AddParameter("state", state);
                    }
                };

            return performRequest<Dictionary<string, CityState>>("areaCodes", Method.POST, false, addParameters).Data;
        }

        /// <summary>
        ///  Cancels a fax
        /// </summary>
        /// <param name="faxId">The id of the fax to cancel.</param>
        /// <param name="state">A two character state or province abbreviation (e.g. IL or YT).
        /// Will only return area codes available for this state.</param>
        /// <returns>A bool indicating whether the operation was successful.</returns>
        public bool CancelFax (int faxId)
        {
            Action<IRestRequest> addParameters = req =>
            {
                req.AddParameter("id", faxId);
            };

            return performRequest<Object>("faxCancel", Method.GET, true, addParameters).Success;
        }

        private Response<T> performRequest<T>(string resource, Method method)
        {
            return performRequest<T>(resource, method, true, r => { });
        }

        private Response<T> performRequest<T>(string resource, Method method, bool auth, Action<IRestRequest> requestModifier)
        {
            var request = new RestRequest();

            if (auth)
            {
                request.AddParameter(KeyName, key);
                request.AddParameter(SecretName, secret);
            }

            // Run any custom modifications
            requestModifier(request);

            request.Method = method;
            request.Resource = resource;

            var response = client.Execute<Response<T>>(request);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response. Check inner exception.";
                var phaxioException = new ApplicationException(message, response.ErrorException);
                throw phaxioException;
            }

            if (!response.Data.Success)
            {
                throw new ApplicationException(response.Data.Message);
            }

            return response.Data;
        }
    }
}