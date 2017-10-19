using Moq;
using Phaxio.ThinRestClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Phaxio.Tests.Helpers
{
    public class RestClientBuilder
    {
        public const string TEST_KEY = "key";
        public const string TEST_SECRET = "secret";

        private string content { get; set; }
        private string contentType { get; set; }
        private byte[] rawBytes { get; set; }
        private HttpStatusCode statusCode { get; set; }
        private Action<IRestRequest> requestAsserts { get; set; }

        public RestClientBuilder WithRequestAsserts(Action<IRestRequest> requestAsserts)
        {
            this.requestAsserts = requestAsserts;

            return this;
        }

        public RestClientBuilder Content(string content)
        {
            this.content = content;

            return this;
        }

        public RestClientBuilder RawBytes(byte[] rawBytes)
        {
            this.rawBytes = rawBytes;

            return this;
        }

        public RestClientBuilder AsText()
        {
            contentType = "text/plain";

            return this;
        }

        public RestClientBuilder AsPng()
        {
            contentType = "image/png";

            return this;
        }

        public RestClientBuilder AsPdf()
        {
            contentType = "application/pdf";

            return this;
        }

        public RestClientBuilder AsJson()
        {
            contentType = "application/json";

            return this;
        }

        public RestClientBuilder Ok()
        {
            statusCode = HttpStatusCode.OK;

            return this;
        }

        public RestClientBuilder RateLimited()
        {
            statusCode = (HttpStatusCode)429;

            return this;
        }

        public RestClientBuilder InvalidEntity()
        {
            statusCode = (HttpStatusCode)422;

            return this;
        }

        public RestClientBuilder Unauthorized()
        {
            statusCode = HttpStatusCode.Unauthorized;

            return this;
        }

        public RestClientBuilder NotFound()
        {
            statusCode = HttpStatusCode.NotFound;

            return this;
        }

        public RestClientBuilder InternalServerError()
        {
            statusCode = HttpStatusCode.InternalServerError;

            return this;
        }

        public IRestClient Build()
        {
            var mockIRestClient = new Mock<IRestClient>();

            mockIRestClient.Setup(x => x.Execute(It.IsAny<IRestRequest>()))
                .Returns<IRestRequest>(req =>
                {
                    requestAsserts?.Invoke(req);

                    var response = new RestResponse();
 
                    response.ContentType = contentType;
                    response.Content = content;
                    response.StatusCode = statusCode;
                    response.RawBytes = rawBytes;

                    return response;
                });

            return mockIRestClient.Object;
        }

        public IRestClient Build<T>()
        {
            var mockIRestClient = new Mock<IRestClient>();

            mockIRestClient.Setup(x => x.Execute<T>(It.IsAny<IRestRequest>()))
                .Returns<IRestRequest>(req =>
                {
                    requestAsserts?.Invoke(req);

                    var response = new RestResponse<T>();

                    response.ContentType = contentType;
                    response.Content = content;
                    response.StatusCode = statusCode;
                    response.RawBytes = rawBytes;

                    return response;
                });

            return mockIRestClient.Object;
        }
    }
}
