using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phaxio.Repositories.V2
{
    public class PublicRepository
    {
        private PhaxioContext client;

        public PublicRepository(PhaxioContext client)
        {
            this.client = client;
            AreaCode = new AreaCodeRepository(client);
            SupportedCountry = new SupportedCountryRepository(client);
        }

        public AreaCodeRepository AreaCode { get; private set; }
        public SupportedCountryRepository SupportedCountry { get; private set; }
    }
}
