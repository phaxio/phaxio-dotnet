using Phaxio.Entities.Internal;
using System.Collections.Generic;

namespace Phaxio.Entities
{
    public class FaxOptions
    {
        public FaxOptions ()
        {
            Tags = new Dictionary<string, string>();
        }

        /// <summary>
        /// Text that will be displayed at the top of each page of the fax. 50 characters maximum. Default header text is "-".
        /// </summary>
        [SerializeAsAttribute("header_text")]
        public string HeaderText { get; set; }

        /// <summary>
        /// A string of html, plain text, or a URL. If additional files are specified as well, this data will be included first in the fax.
        /// </summary>
        [SerializeAsAttribute("string_data")]
        public string StringData { get; set; }
            
        /// <summary>
        /// Can be 'html', 'url', or 'text'. If not specified, default is 'text'. See string data rendering for more info.
        /// </summary>
        [SerializeAsAttribute("string_data_type")]
        public string StringDataType { get; set; }
        
        /// <summary>
        /// If present and true, fax will be sent in batching mode. Requires BatchDelaySeconds to be specified. See batching for more info.
        /// </summary>
        [SerializeAsAttribute("batch")]
        public bool? IsBatch { get; set; }
        
        /// <summary>
        /// The amount of time, in seconds, before the batch is fired. Must be specified if IsBatch=true. Maximum delay is 3600 (1 hour).
        /// </summary>
        [SerializeAsAttribute("batch_delay")]
        public int? BatchDelaySeconds { get; set; }
        
        /// <summary>
        /// If true when batch=true, fax will be blocked until the receiving machine is no longer busy. See batching for more info.
        /// </summary>
        [SerializeAsAttribute("batch_collision_avoidance")]
        public bool? AvoidBatchCollision { get; set; }
        
        /// <summary>
        /// You can specify a callback url that will override the one you have defined globally for your account.
        /// </summary>
        [SerializeAsAttribute("callback_url")]
        public string CallbackUrl { get; set; }
        
        /// <summary>
        /// A number of minutes after which the fax will be canceled if it hasn't yet completed. Must be between 3 and 60.
        /// Additionally, for faxes with a BatchDelaySeconds, the CancelTimeoutAfter must be at least 3 minutes after the batch_delay.
        /// </summary>
        [SerializeAsAttribute("cancel_timeout")]
        public int? CancelTimeoutAfter { get; set; }
        
        /// <summary>
        /// Tags for your fax. You may specify a maximum of 10.
        /// </summary>
        public Dictionary<string, string> Tags { get; set; }
        
        /// <summary>
        /// A Phaxio phone number you would like to use for the caller id, in E.164 format (+[country code][number])
        /// </summary>
        [SerializeAsAttribute("caller_id")]
        public string CallerId { get; set; }
        
        /// <summary>
        /// When using a test API key, this will simulate a sending failure at Phaxio.
        /// </summary>
        [SerializeAsAttribute("test_fail")]
        public string FailureErrorType { get; set; }
    }
}
