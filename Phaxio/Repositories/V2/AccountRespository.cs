using Phaxio.Entities.V2;
using Phaxio.ThinRestClient;

namespace Phaxio.Repositories.V2
{
    public class AccountRespository
    {
        private PhaxioContext client;

        public AccountRespository(PhaxioContext client)
        {
            this.client = client;
        }

        /// <summary>
        ///  Gets the account for this Phaxio instance.
        /// </summary>
        /// <returns>An AccountStatus object</returns>
        public AccountStatus Status
        {
            get
            {
                return client.request<AccountStatus>("account/status", Method.GET).Data;
            }
        }
    }
}
