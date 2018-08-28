using Phaxio.ThinRestClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Phaxio.Entities;

namespace Phaxio.Resources.V2
{
    public class Fax
    {
        private PhaxioClient _phaxioClient;
        public PhaxioClient PhaxioClient {
            private get
            {
                return _phaxioClient;
            }
            set
            {
                _phaxioClient = value;
                File.PhaxioClient = value;
            }
        }

        public Fax(int id)
        {
            File = new FaxFile(id);
            Id = id;
        }

        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "barcodes")]
        public IEnumerable<Barcode> Barcodes { get; set; }

        [JsonProperty(PropertyName = "caller_name")]
        public string CallerName { get; set; }

        [JsonProperty(PropertyName = "direction")]
        public string Direction { get; set; }

        [JsonProperty(PropertyName = "num_pages")]
        public int NumPages { get; set; }

        [JsonProperty(PropertyName = "cost")]
        public int Cost { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "is_test")]
        public bool IsTest { get; set; }

        [JsonProperty(PropertyName = "created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty(PropertyName = "from_number")]
        public string FromNumber { get; set; }

        [JsonProperty(PropertyName = "to_number")]
        public string ToNumber { get; set; }

        [JsonProperty(PropertyName = "recipients")]
        public IEnumerable<Recipient> Recipients { get; set; }

        [JsonProperty(PropertyName = "tags")]
        public Dictionary<string, string> Tags { get; set; }

        [JsonProperty(PropertyName = "error_type")]
        public string ErrorType { get; set; }

        [JsonProperty(PropertyName = "error_id")]
        public int? ErrorId { get; set; }

        [JsonProperty(PropertyName = "error_message")]
        public string ErrorMessage { get; set; }

        [JsonProperty(PropertyName = "completed_at")]
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        ///  The fax's file
        /// </summary>
        public FaxFile File { get; private set; }

        /// <summary>
        ///  Resends this fax
        /// </summary>
        /// <param name="callbackUrl">The URL to send the callback to.</param>
        public void Resend(string callbackUrl = null)
        {
            Action<IRestRequest> addParameters = req =>
            {
                if (callbackUrl != null)
                {
                    req.AddParameter("callback_url", callbackUrl);
                }
            };

            PhaxioClient.request<Object>("faxes/" + Id + "/resend", Method.POST, true, addParameters).ToResult();
        }

        /// <summary>
        ///  Cancels this fax
        /// </summary>
        public void Cancel()
        {
            PhaxioClient.request<Object>("faxes/" + Id + "/cancel", Method.POST).ToResult();
        }

        /// <summary>
        ///  Deletes this fax
        /// </summary>
        public void Delete()
        {
            PhaxioClient.request<Object>("faxes/" + Id, Method.DELETE).ToResult();
        }
    }
}
