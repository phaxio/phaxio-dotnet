using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phaxio.Entities.Internal
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class SerializeAsAttribute : System.Attribute
    {
        public string Value;

        public SerializeAsAttribute (string value)
        {
            this.Value = value;
        }
    }
}