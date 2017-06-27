using Phaxio.Clients.Internal;
using Phaxio.Errors.V2;
using Phaxio.Repositories.V2;
using Phaxio.ThinRestClient;
using Phaxio.ThinRestClient.Helpers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Phaxio
{
    public class PhaxioClient : BasePhaxioClient
    {
        private const string phaxioApiEndpoint = "https://api.phaxio.com/v2/";

        public PhaxioClient(string key, string secret)
            : this(key, secret, new RestClient())
        {
        }

        public PhaxioClient(string key, string secret, IRestClient restClient)
            : base(key, secret, new RestClient())
        {
            // Initialize the rest client
            client = restClient;
            client.BaseUrl = new Uri(phaxioApiEndpoint);

            Fax = new FaxRepository(this);
            Public = new PublicRepository(this);
            PhoneNumber = new PhoneNumberRepository(this);
            PhaxCode = new PhaxCodeRepository(this);
            Account = new AccountRespository(this);
        }

        public AccountRespository Account { get; private set; }
        public FaxRepository Fax { get; private set; }
        public PublicRepository Public { get; private set; }
        public PhaxCodeRepository PhaxCode { get; private set; }
        public PhoneNumberRepository PhoneNumber { get; private set; }

        internal Response<List<T>> requestList<T>(string resource, Method method, bool auth, Action<IRestRequest> requestModifier)
        {
            var response = request<List<T>>(resource, method, auth, requestModifier);

            foreach (var item in response.Data)
            {
                modifyDataItem(item);
            }

            return response;
        }

        internal IEnumerable<T> pagedRequest<T>(string resource, Method method, bool auth, Action<IRestRequest> requestModifier)
        {
            var response = requestList<T>(resource, method, auth, requestModifier);

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
        protected override void checkException(IRestResponse response)
        {
            handleNetworkException(response);

            if (response.ContentType == "application/json")
            {
                var json = new JsonDeserializer();

                var phaxioException = json.Deserialize<Response<object>>(response);

                handlePhaxioException(response, phaxioException);
            }
        }

        protected override void checkException<T>(IRestResponse<Response<T>> response)
        {
            handleNetworkException(response);
            handlePhaxioException(response, response.Data);
        }

        protected override void modifyDataItem<T>(T item)
        {
            PropertyInfo property = typeof(T).GetProperty("PhaxioClient");

            if (property != null)
            {
                property.SetValue(item, this, null);
            }
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
    }
}
