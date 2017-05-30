using Phaxio.ThinRestClient;
using Phaxio.Entities;
using System;
using System.Collections.Generic;

namespace Phaxio.Repositories.V2
{
    public class AreaCodeRepository
    {
        private PhaxioContext client;

        public AreaCodeRepository(PhaxioContext client)
        {
            this.client = client;
        }

        public IEnumerable<AreaCode> List(bool? tollFree = null,
            string countryCode = null,
            string country = null,
            string state = null,
            int? perPage = null,
            int? page = null
        )
        {
            Action<IRestRequest> addParameters = req =>
            {
                if (tollFree != null)
                {
                    req.AddParameter("toll_free", tollFree);
                }

                if (countryCode != null)
                {
                    req.AddParameter("country_code", countryCode);
                }

                if (country != null)
                {
                    req.AddParameter("country", country);
                }

                if (state != null)
                {
                    if (country == null && countryCode == null)
                    {
                        throw new ArgumentException("You must supply either a country code or a country if you specify a state.");
                    }

                    req.AddParameter("state", state);
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

            return client.pagedRequest<AreaCode>("public/area_codes", Method.GET, false, addParameters);
        }
    }
}
