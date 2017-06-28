using Phaxio.Resources.V2;
using Phaxio.ThinRestClient;
using System;
using System.Net;

namespace Phaxio.Repositories.V2
{
    public class PhaxCodeRepository
    {
        private PhaxioClient client;

        public PhaxCodeRepository(PhaxioClient client)
        {
            this.client = client;
        }

        /// <summary>
        ///  Creates a PhaxCode and returns an identifier for the barcode image.
        /// </summary>
        /// <param name="metadata">Metadata to associate with this code.</param>
        /// <returns>A PhaxCode object.</returns>
        public PhaxCode Create(string metadata)
        {
            Action<IRestRequest> addParameters = req =>
            {
                req.AddParameter("metadata", metadata);
            };

            return client.request<PhaxCode>("phax_codes.json", Method.POST, true, addParameters).Data;
        }

        /// <summary>
        /// Retrieves a PhaxCode
        /// </summary>
        /// <param name="id">The id of the PhaxCode. If none is passed in, the default PhaxCode is downloaded.</param>
        /// <returns>a PhaxCode object.</returns>
        public PhaxCode Retrieve(string id = null)
        {
            var resource = "phax_code";

            if (id != null)
            {
                resource += "s/" + WebUtility.UrlEncode(id);
            }

            return client.request<PhaxCode>(resource + ".json", Method.GET).Data;
        }
    }
}
