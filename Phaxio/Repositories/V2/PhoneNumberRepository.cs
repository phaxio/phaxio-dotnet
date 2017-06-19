using Phaxio.Resources.V2;
using Phaxio.ThinRestClient;
using System;
using System.Collections.Generic;

namespace Phaxio.Repositories.V2
{
    public class PhoneNumberRepository
    {
        private PhaxioClient client;

        public PhoneNumberRepository(PhaxioClient client)
        {
            this.client = client;
        }

        /// <summary>
        ///  Provisions a new fax number
        /// </summary>
        /// <param name="countryCode">The country code to provsion the number in.</param>
        /// <param name="areaCode">The area code to provsion the number in.</param>
        /// <param name="callbackUrl">The URL that Phaxio will post to when a fax is recieved at this number.</param>
        /// <returns>A PhoneNumber object representing the new number.</returns>
        public PhoneNumber Create(string areaCode, string countryCode, string callbackUrl = null)
        {
            Action<IRestRequest> addParameters = req =>
            {
                req.AddParameter("country_code", countryCode);
                req.AddParameter("area_code", areaCode);

                if (callbackUrl != null)
                {
                    req.AddParameter("callback_url", callbackUrl);
                }
            };

            return client.request<PhoneNumber>("phone_numbers", Method.POST, true, addParameters).Data;
        }

        /// <summary>
        ///  Lists the phone numbers provisioned to your account.
        /// </summary>
        /// <param name="countryCode">The country code to search (e.g., "1" for US).</param>
        /// <param name="country">The area code search. If this is specified, you must specify the country code.</param>
        /// <param name="perPage">The maximum number of results to return per call or "page" (1000 max).</param>
        /// <param name="page">The page number to return for the request. 1-based.</param>
        /// <returns>A IEnumerable&lt;PhoneNumberV2&gt;</returns>
        public IEnumerable<PhoneNumber> List(string areaCode = null,
            string countryCode = null,
            int? perPage = null,
            int? page = null)
        {
            Action<IRestRequest> addParameters = req =>
            {
                if (countryCode != null)
                {
                    req.AddParameter("country_code", countryCode);
                }

                if (areaCode != null)
                {
                    if (countryCode == null)
                    {
                        throw new ArgumentException("You must supply either a country code or a country if you specify a state.");
                    }

                    req.AddParameter("area_code", areaCode);
                }

                if (perPage != null)
                {
                    req.AddParameter("per_page", perPage);
                }

                if (page != null)
                {
                    req.AddParameter("page", page);
                }
            };

            return client.pagedRequest<PhoneNumber>("phone_numbers", Method.GET, true, addParameters);
        }

        /// <summary>
        ///  Gets the information for a phone number
        /// </summary>
        /// <returns>A PhoneNumber object</returns>
        public PhoneNumber Retrieve(string number)
        {
            return client.request<PhoneNumber>("phone_numbers/" + number, Method.GET).Data;
        }
    }
}
