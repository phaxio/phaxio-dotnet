using Phaxio.Entities.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phaxio.Entities
{
    public class FaxRequest
    {
        public FaxRequest()
        {
            Tags = new Dictionary<string, string>();
            ToNumbers = new List<string>();
            ContentUrls = new List<string>();
            Files = new List<FileInfo>();
        }

        /// <summary>
        /// The number (with country code) to send the fax to. Ignored if ToNumbers is set.
        /// </summary>
        public string ToNumber { get; set; }

        /// <summary>
        /// The numbers (with country code) to send the fax to.
        /// </summary>
        public IEnumerable<string> ToNumbers { get; set; }

        /// <summary>
        /// A URL to be rendered and sent as the fax content. Ignored if ContentUrls is set.
        /// </summary>
        public string ContentUrl { get; set; }

        /// <summary>
        /// A list of URLs to be rendered and sent as the fax content.
        /// </summary>
        public IEnumerable<string> ContentUrls { get; set; }

        /// <summary>
        /// The file to send. Supports doc, docx, pdf, tif, jpg, odt, txt, html and png. Ignored if Files is set.
        /// </summary>
        public FileInfo File { get; set; }

        /// <summary>
        /// The files to send. Supports doc, docx, pdf, tif, jpg, odt, txt, html and png. Ignored if Files is set.
        /// </summary>
        public IEnumerable<FileInfo> Files { get; set; }

        /// <summary>
        /// Text that will be displayed at the top of each page of the fax. 50 characters maximum. Default header text is "-".
        /// </summary>
        [SerializeAs("header_text")]
        public string HeaderText { get; set; }

        /// <summary>
        /// Enables batching and specifies the amount of time, in seconds, before the batch is fired. Maximum delay is 3600 (1 hour).
        /// </summary>
        [SerializeAs("batch_delay")]
        public int? BatchDelaySeconds { get; set; }

        /// <summary>
        /// When BatchDelaySeconds is set, fax will be blocked until the receiving machine is no longer busy.
        /// </summary>
        [SerializeAs("batch_collision_avoidance")]
        public bool? AvoidBatchCollision { get; set; }

        /// <summary>
        /// You can specify a callback url that will override the one you have defined globally for your account.
        /// </summary>
        [SerializeAs("callback_url")]
        public string CallbackUrl { get; set; }

        /// <summary>
        /// A number of minutes after which the fax will be canceled if it hasn't yet completed. Must be between 3 and 60.
        /// Additionally, for faxes with a BatchDelaySeconds, the CancelTimeoutAfter must be at least 3 minutes after the batch_delay.
        /// </summary>
        [SerializeAs("cancel_timeout")]
        public int? CancelTimeoutAfter { get; set; }

        /// <summary>
        /// Tags for your fax. You may specify a maximum of 10.
        /// </summary>
        public Dictionary<string, string> Tags { get; set; }

        /// <summary>
        /// A Phaxio phone number you would like to use for the caller id, in E.164 format (+[country code][number])
        /// </summary>
        [SerializeAs("caller_id")]
        public string CallerId { get; set; }

        /// <summary>
        /// When using a test API key, this will simulate a sending failure at Phaxio.
        /// </summary>
        [SerializeAs("test_fail")]
        public string FailureErrorType { get; set; }
    }
}