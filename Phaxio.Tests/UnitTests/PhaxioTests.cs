using NUnit.Framework;
using Phaxio.Tests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phaxio.Tests
{
    [TestFixture]
    public class PhaxioTests
    {
        [Test]
        public void UnitTests_ValidRequestWorks()
        {
            var clientBuilder = new IRestClientBuilder { Op = "accountStatus" };
            var phaxio = new Phaxio(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            var account = phaxio.GetAccountStatus();

            var expectedAccount = PocoFixtures.GetTestAccount();
        }

        [Test]
        public void UnitTests_InvalidKeyThrowsException()
        {
            var clientBuilder = new IRestClientBuilder { Op = "accountStatus" };
            var phaxio = new Phaxio("bad_key", IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            Assert.Throws( typeof(ApplicationException), () => phaxio.GetAccountStatus());
        }

        [Test]
        public void UnitTests_InvalidSecretThrowsException()
        {
            var clientBuilder = new IRestClientBuilder { Op = "accountStatus" };
            var phaxio = new Phaxio(IRestClientBuilder.TEST_KEY, "bad_secret", clientBuilder.Build());

            Assert.Throws(typeof(ApplicationException), () => phaxio.GetAccountStatus());
        }
    }
}
