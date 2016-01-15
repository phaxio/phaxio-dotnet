using Moq;
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
    class MockPhaxioService
    {
        public const string TEST_KEY = "key";
        public const string TEST_SECRET = "secret";

        public static IRestClient GetRestClient (string op)
        {
            var mockIRestClient = new Mock<IRestClient>();

            string content = null;

            if (op == "accountStatus")
            {
                content = @"{
                                ""success"":true,
                                ""message"":""Account status retrieved successfully"",
                                ""data"": {
                                    ""faxes_sent_this_month"":120,
                                    ""faxes_sent_today"":10,
                                    ""balance"":3000
                                }
                            }";

                setup<Account>(content, mockIRestClient);
            }

            return mockIRestClient.Object;
        }

        private static void setup<T> (string content, Mock<IRestClient> mockIRestClient)
        {
            JsonDeserializer json = new JsonDeserializer();

            var response = json.Deserialize<Response<Account>>(new RestResponse { Content = content });

            mockIRestClient.Setup(x => x.Execute<Response<Account>>(It.IsAny<IRestRequest>()))
                .Returns<IRestRequest>(req => validateCreds(req, response));
        }

        private static RestResponse<Response<T>> validateCreds<T>(IRestRequest request, Response<T> obj)
        {
            if ((string)request.Parameters[0].Value != TEST_KEY || (string)request.Parameters[1].Value != TEST_SECRET)
            {
                obj.Success = false;
                obj.Message = "Account keys were invalid.";
                obj.Data = default(T);
            }

            return new RestResponse<Response<T>>
            {
                Data = obj,
                StatusCode = HttpStatusCode.OK
            };
        }

        public static Account GetTestAccount ()
        {
            return new Account
            {
                FaxesSentThisMonth = 120,
                FaxesSentToday = 10,
                Balance = 3000
            };
        }
    }
}
