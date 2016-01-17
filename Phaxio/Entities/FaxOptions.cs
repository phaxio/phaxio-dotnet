using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phaxio.Entities
{

    public class FaxOptions
    {
        public string HeaderText { get; set; }
        public string StringData { get; set; }
        public string StringDataType { get; set; }
        public bool IsBatch { get; set; }
        public int BatchDelay { get; set; }
        public bool AvoidBatchCollision { get; set; }
        public string CallbackUrl { get; set; }
        public int CancelTimeout { get; set; }
        public Dictionary<string, string> Tags { get; set; }
        public PhoneNumber CallerId { get; set; }
        public string FailureErrorType { get; set; }
    }
}
