using System;
using System.Collections.Generic;
using System.IO;

namespace Phaxio.ThinRestClient
{
    public interface IRestRequest
    {
        Method Method { get; set; }
        string Resource { get; set; }
        List<Parameter> Parameters { get; set; }
        List<FileParameter> Files { get; set; }
        IRestRequest AddParameter(string name, object value);
        IRestRequest AddFile(string name, byte[] bytes, string fileName, string contentType = null);
        Action<Stream> ResponseWriter { get; set; }
    }
}