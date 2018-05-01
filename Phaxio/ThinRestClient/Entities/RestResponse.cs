using Phaxio.ThinRestClient.Helpers;
using System;
using System.Net;
using System.Net.Http;

namespace Phaxio.ThinRestClient
{
    public class RestResponse : IRestResponse
    {
        internal HttpResponseMessage httpResponse;

        public RestResponse() { }

        internal RestResponse (HttpResponseMessage httpResponse)
        {
            this.httpResponse = httpResponse;
        }

        private string _contentType { get; set; }
        public string ContentType
        {
            get
            {
                if (_contentType == null && httpResponse != null)
                {
					_contentType = httpResponse.Content.Headers.ContentType.MediaType;
                }

                return _contentType;
            }
            set
            {
                _contentType = value;
            }
        }

        private HttpStatusCode? _statusCode;
        public HttpStatusCode StatusCode
        {
            get
            {
                if (_statusCode == null && httpResponse != null)
                {
                    _statusCode = httpResponse.StatusCode;
                }

                return _statusCode.Value;
            }
            set
            {
                _statusCode = value;
            }
        }

        public Exception ErrorException { get; set; }

        private string _content;
        public string Content
        {
            get
            {
                if (_content == null && httpResponse != null)
                {
                    _content = httpResponse.Content.ReadAsStringAsync().Result;
                }

                return _content;
            }
            set
            {
                _content = value;
            }
        }

        private byte[] _rawBytes;
        public byte[] RawBytes
        {
            get
            {
                if (_rawBytes == null && httpResponse != null)
                {
                    _rawBytes = httpResponse.Content.ReadAsByteArrayAsync().Result;
                }

                return _rawBytes;
            }
            set
            {
                _rawBytes = value;
            }
        }
    }

    public class RestResponse<T> : RestResponse, IRestResponse<T>
    {
        private T _data { get; set; }
        public T Data
        {
            get
            {
                if (_data == null && Content != null)
                {
                    JsonDeserializer deserializer = new JsonDeserializer();
                    _data = deserializer.Deserialize<T>(this);
                }

                return _data;
            }
            set
            {
                _data = value;
            }
        }

        public RestResponse() { }

        internal RestResponse(HttpResponseMessage httpResponse)
            : base(httpResponse)
        {
        }
    }
}