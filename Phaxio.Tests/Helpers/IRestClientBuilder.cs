using Moq;
using Phaxio.Entities;
using Phaxio.Entities.Internal;
using Phaxio.Tests.Helpers;
using Phaxio.ThinRestClient;
using System;
using System.Collections.Generic;
using System.Net;

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

            // Make Build() generic
            if (Op == "accountStatus")
            {
                typedSetup<Account>(content, mockIRestClient);
            }
            else if (Op == "areaCodes")
            {
                typedSetup<Dictionary<string, CityState>>(content, mockIRestClient);
            }
            else if (Op == "faxCancel")
            {
                typedSetup<Object>(content, mockIRestClient);
            }
            else if (Op == "resendFax")
            {
                typedSetup<Object>(content, mockIRestClient);
            }
            else if (Op == "deleteFax")
            {
                typedSetup<Object>(content, mockIRestClient);
            }
            else if (Op == "releaseNumber")
            {
                typedSetup<Object>(content, mockIRestClient);
            }
            else if (Op == "testReceive")
            {
                typedSetup<Object>(content, mockIRestClient);
            }
            else if (Op == "supportedCountries")
            {
                typedSetup<Dictionary<string, Pricing>>(content, mockIRestClient);
            }
            else if (Op == "provisionNumber")
            {
                typedSetup<PhoneNumber>(content, mockIRestClient);
            }
            else if (Op == "numberList")
            {
                typedSetup<List<PhoneNumber>>(content, mockIRestClient);
            }
            else if (Op == "createPhaxCodeUrl")
            {
                typedSetup<Url>(content, mockIRestClient);
            }
            else if (Op == "send")
            {
                typedSetup<dynamic>(content, mockIRestClient);
            }
            
            return mockIRestClient.Object;
        }

        public IRestClient BuildUntyped ()
        {
            var mockIRestClient = new Mock<IRestClient>();

            if (Op == "createPhaxCodeDownload")
            {
                mockIRestClient.Setup(x => x.Execute(It.IsAny<IRestRequest>()))
                .Returns<IRestRequest>(req =>
                {
                    var response = new RestResponse();
                    var authFailed = false;

                    checks(req, () =>
                    {
                        authFailed = true;
                    });

                    if (authFailed)
                    {
                        response.ContentType = "application/json";
                        response.Content = JsonResponseFixtures.Fixtures["authFail"];
                    }
                    else
                    {
                        response.ContentType = "image/png";
                        response.RawBytes = BinaryFixtures.GetTestPhaxCode();
                    }

                    return response;
                });
            }
            else if (Op == "attachPhaxCodeToPdf")
            {
                mockIRestClient.Setup(x => x.Execute(It.IsAny<IRestRequest>()))
                .Returns<IRestRequest>(req =>
                {
                    var response = new RestResponse();
                    var authFailed = false;

                    checks(req, () =>
                    {
                        authFailed = true;
                    });

                    if (authFailed)
                    {
                        response.ContentType = "application/json";
                        response.Content = JsonResponseFixtures.Fixtures["authFail"];
                    }
                    else
                    {
                        response.ContentType = "application/pdf";
                        response.RawBytes = BinaryFixtures.GetTestPdf();
                    }

                    return response;
                });
            }
            else if (Op == "attachPhaxCodeToPdfStream")
            {
                mockIRestClient.Setup(x => x.Execute(It.IsAny<IRestRequest>()))
                .Returns<IRestRequest>(req =>
                {
                    var response = new RestResponse();
                    var authFailed = false;

                    checks(req, () =>
                    {
                        authFailed = true;
                    });

                    if (authFailed)
                    {
                        response.ContentType = "application/json";
                        response.Content = JsonResponseFixtures.Fixtures["authFail"];
                    }
                    else
                    {
                        response.ContentType = "application/pdf";
                        var writer = req.ResponseWriter;
                        var testPdf = BinaryFixtures.getTestPdfFile();
                        writer(testPdf.OpenRead());
                    }

                    return response;
                });
            }
            else if (Op == "faxFile")
            {
                mockIRestClient.Setup(x => x.Execute(It.IsAny<IRestRequest>()))
                .Returns<IRestRequest>(req =>
                {
                    var response = new RestResponse();
                    var authFailed = false;

                    checks(req, () =>
                    {
                        authFailed = true;
                    });

                    if (authFailed)
                    {
                        response.ContentType = "application/json";
                        response.Content = JsonResponseFixtures.Fixtures["authFail"];
                    }
                    else
                    {
                        response.ContentType = "application/pdf";
                        response.RawBytes = BinaryFixtures.GetTestPdf();
                    }

                    return response;
                });
            }
            else if (Op == "getHostedDocument")
            {
                mockIRestClient.Setup(x => x.Execute(It.IsAny<IRestRequest>()))
                .Returns<IRestRequest>(req =>
                {
                    var response = new RestResponse();
                    var authFailed = false;

                    checks(req, () =>
                    {
                        authFailed = true;
                    });

                    if (authFailed)
                    {
                        response.ContentType = "application/json";
                        response.Content = JsonResponseFixtures.Fixtures["authFail"];
                    }
                    else
                    {
                        response.ContentType = "application/pdf";
                        response.RawBytes = BinaryFixtures.GetTestPdf();
                    }

                    return response;
                });
            }
            
            return mockIRestClient.Object;
        }

        private void typedSetup<T> (string content, Mock<IRestClient> mockIRestClient)
        {
            JsonDeserializer json = new JsonDeserializer();

            var response = json.Deserialize<Response<T>>(new RestResponse { Content = content });

            mockIRestClient.Setup(x => x.Execute<Response<T>>(It.IsAny<IRestRequest>()))
                .Returns<IRestRequest>(req => typedRespond(req, response));
        }

        private RestResponse<Response<T>> typedRespond<T>(IRestRequest request, Response<T> obj)
        {
            checks(request, () => {
                obj.Success = false;
                obj.Message = "Account keys were invalid.";
                obj.Data = default(T);
            });
                
            return new RestResponse<Response<T>>
            {
                Data = obj,
                StatusCode = HttpStatusCode.OK
            };
        }

        private void checks (IRestRequest request, Action onFailure)
        {
            if (!NoAuth)
            {
                var parameters = ParametersHelper.ToDictionary(request.Parameters);

                if ((string)parameters[PhaxioConstants.KEY_NAME] != TEST_KEY
                    || (string)parameters[PhaxioConstants.SECRET_NAME] != TEST_SECRET)
                {
                    onFailure();
                }
            }

            RequestAsserts?.Invoke(request);
        }
    }
}
