using System;

namespace Phaxio.Entities
{
    /// <summary>
    ///  This represents a PhaxCode
    /// </summary>
    public class PhaxCode
    {
        public string Identifier { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Metadata { get; set; }
    }
}
