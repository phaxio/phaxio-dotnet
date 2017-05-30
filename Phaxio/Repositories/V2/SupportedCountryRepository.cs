using Phaxio.Entities;
using Phaxio.ThinRestClient;
using System;
using System.Collections.Generic;

namespace Phaxio.Repositories.V2
{
    public class SupportedCountryRepository
    {
        private PhaxioContext client;

        public SupportedCountryRepository(PhaxioContext client)
        {
            this.client = client;
        }

        /// <summary>
        /// Lists supported countries with pricing
        /// </summary>
        /// <param name="perPage">The maximum number of results to return per call or "page" (1000 max).</param>
        /// <param name="page">The page number to return for the request. 1-based.</param>
        /// <returns>A IEnumerable&lt;Country&gt;</returns>
        public IEnumerable<Country> List(int? perPage = null,
            int? page = null)
        {
            Action<IRestRequest> addParameters = req =>
            {
                if (perPage != null)
                {
                    req.AddParameter("per_page", perPage);
                }

                if (page != null)
                {
                    req.AddParameter("page", page);
                }
            };

           return client.pagedRequest<Country>("public/countries", Method.GET, true, addParameters);
        }
    }
}
