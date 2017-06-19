using System;
using System.Collections.Generic;
using System.IO;

namespace Phaxio.ThinRestClient
{
    class RestRequest : IRestRequest
    {
        public List<FileParameter> Files { get; set; }

        public Method Method { get; set; }

        public string Resource { get; set; }

        public List<Parameter> Parameters { get; set; }

        public Action<Stream> ResponseWriter { get; set; }

        public RestRequest ()
        {
            Parameters = new List<Parameter>();
            Files = new List<FileParameter>();
        }

        public IRestRequest AddFile(string name, byte[] bytes, string fileName, string contentType = null)
        {
            Files.Add(new FileParameter { Name = name, Bytes = bytes, FileName = fileName, ContentType = contentType });

            return this;
        }

        public IRestRequest AddParameter(string name, object value)
        {
            var existingParameter = Parameters.Find(p => p.Name == name);

            if (existingParameter != null)
            {
                existingParameter.Value = value;
            }
            else
            {
                Parameters.Add(new Parameter { Name = name, Value = value });
            }

            return this;
        }
    }
}