using System;
using System.Net;

namespace Phaxio.ThinRestClient
{
    public interface IRestResponse
    {
        string Content { get; set; }
        string ContentType { get; set; }
        HttpStatusCode StatusCode { get; set; }
        Exception ErrorException { get; set; }
        byte[] RawBytes { get; set; }
    }

    public interface IRestResponse<T> : IRestResponse
    {
        T Data { get; set; }
    }
}