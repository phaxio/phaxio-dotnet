using System;

namespace Phaxio.Entities.Internal
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SerializeAsAttribute : Attribute
    {
        public string Value;

        public SerializeAsAttribute (string value)
        {
            Value = value;
        }
    }
}