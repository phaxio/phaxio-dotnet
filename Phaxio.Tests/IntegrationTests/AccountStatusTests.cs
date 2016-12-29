using NUnit.Framework;
using Phaxio.Tests.Helpers;

namespace Phaxio.Tests.IntegrationTests
{
    [TestFixture, Explicit]
    class AccountStatusTests
    {
        [Test]
        public void IntegrationTests_AccountStatus_Get()
        {
            var config = new KeyManager();

            var phaxio = new PhaxioClient(config["api_key"], config["api_secret"]);

            var account = phaxio.GetAccountStatus();
        }
    }
}
