using Phaxio.ThinRestClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phaxio.Tests.Helpers
{
    class ParametersHelper
    {
        public static Dictionary<string, object> ToDictionary (List<Parameter> parameters)
        {
            var dict = new Dictionary<string, object>();

            foreach (var param in parameters)
            {
                dict.Add(param.Name, param.Value);
            }

            return dict;
        }
    }
}
