using NUnit.Framework;
using Phaxio.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phaxio.Tests.IntegrationTests
{
    [TestFixture, Explicit]
    class AccountStatusTests
    {
        [Test]
        public void IntegrationTests_AccountStatus_Get()
        {
            var config = new KeyManager();

            var phaxio = new Phaxio(config["api_key"], config["api_secret"]);

            var account = phaxio.GetAccountStatus();
        }
    }
}
