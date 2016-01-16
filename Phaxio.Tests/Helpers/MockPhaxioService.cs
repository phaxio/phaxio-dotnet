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
                if ((string)request.Parameters[0].Value != TEST_KEY || (string)request.Parameters[1].Value != TEST_SECRET)
                {
                    obj.Success = false;
                    obj.Message = "Account keys were invalid.";
                    obj.Data = default(T);
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
