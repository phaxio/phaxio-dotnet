using Moq;
using Phaxio.Entities;
using Phaxio.Entities.Internal;
using RestSharp;
using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Phaxio.Tests
{
    class IRestClientBuilder
    {
        public const string TEST_KEY = "key";
        public const string TEST_SECRET = "secret";

        public string Op { get; set; }
        public bool NoAuth { get; set; }
        public Action<IRestRequest> RequestAsserts { get; set; }

        public IRestClient Build ()
        {
            var mockIRestClient = new Mock<IRestClient>();

            string content = JsonResponseFixtures.Fixtures[Op];

            if (Op == "accountStatus")
            {
                setup<Account>(content, mockIRestClient);
            }
            else if (Op == "areaCodes")
            {
                setup<Dictionary<string, CityState>>(content, mockIRestClient);
            }
            else if (Op == "faxCancel")
            {
                setup<Object>(content, mockIRestClient);
            }
            else if (Op == "resendFax")
            {
                setup<Object>(content, mockIRestClient);
            }
            else if (Op == "deleteFax")
            {
                setup<Object>(content, mockIRestClient);
            }
            else if (Op == "releaseNumber")
            {
                setup<Object>(content, mockIRestClient);
            }
            else if (Op == "supportedCountries")
            {
                setup<Dictionary<string, Pricing>>(content, mockIRestClient);
            }
            else if (Op == "provisionNumber")
            {
                setup<PhoneNumber>(content, mockIRestClient);
            }
            else if (Op == "numberList")
            {
                setup<List<PhoneNumber>>(content, mockIRestClient);
            }

            return mockIRestClient.Object;
        }

        private void setup<T> (string content, Mock<IRestClient> mockIRestClient)
        {
            JsonDeserializer json = new JsonDeserializer();

            var response = json.Deserialize<Response<T>>(new RestResponse { Content = content });

            mockIRestClient.Setup(x => x.Execute<Response<T>>(It.IsAny<IRestRequest>()))
                .Returns<IRestRequest>(req => respond(req, response));
        }

        private RestResponse<Response<T>> respond<T>(IRestRequest request, Response<T> obj)
        {
            if (!NoAuth)
            {
                foreach (var param in request.Parameters)
                {
                    if (
                            (param.Name == Phaxio.KeyName && (string)param.Value != TEST_KEY)
                            ||
                            (param.Name == Phaxio.SecretName && (string)param.Value != TEST_SECRET)
                       )
                    {
                        obj.Success = false;
                        obj.Message = "Account keys were invalid.";
                        obj.Data = default(T);
                    }
                }
            }

            if (RequestAsserts != null)
            {
                RequestAsserts(request);
            }
                
            return new RestResponse<Response<T>>
            {
                Data = obj,
                StatusCode = HttpStatusCode.OK
            };
        }
    }
}
