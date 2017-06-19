using NUnit.Framework;
using Phaxio.Tests.Helpers;

namespace Phaxio.Tests.IntegrationTests.V2
{
    [TestFixture, Explicit]
    class AccountStatusTests
    {
        [Test]
        public void IntegrationTests_V2_AccountStatus_Get()
        {
            var config = new KeyManager();

            var phaxio = new PhaxioClient(config["api_key"], config["api_secret"]);

            var account = phaxio.Account.Status;
        }
    }
}
