using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Phaxio.Examples.ReceiveCallback.Models
{
    public class Fax
    {
        public string Direction { get; set; }
        public dynamic Json { get; set; }
        public bool IsTest { get; set; }
        public bool Success { get; set; }
        public string Key { get; set; }
    }
}