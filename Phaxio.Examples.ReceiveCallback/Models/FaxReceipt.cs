using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Phaxio.Examples.ReceiveCallback.Models
{
    public class FaxReceipt
    {
        public string Direction { get; set; }
        public dynamic Fax { get; set; }
        public bool IsTest { get; set; }
        public bool Success { get; set; }
        public string Key { get; set; }
    }
}