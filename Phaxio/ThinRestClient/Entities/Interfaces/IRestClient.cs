using System;

namespace Phaxio.ThinRestClient
{
    public interface IRestClient
    {
        Uri BaseUrl { get; set; }
        IRestResponse Execute(IRestRequest request);
        IRestResponse<T> Execute<T>(IRestRequest request);
    }
}
