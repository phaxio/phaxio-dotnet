using NUnit.Framework;
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
            var phaxio = new Phaxio(MockPhaxioService.TEST_KEY, MockPhaxioService.TEST_SECRET, MockPhaxioService.GetRestClient("accountStatus"));

            var account = phaxio.GetAccountStatus();

            var expectedAccount = MockPhaxioService.GetTestAccount();
        }

        [Test]
        public void UnitTests_InvalidKeyThrowsException()
        {
            var phaxio = new Phaxio("bad_key", MockPhaxioService.TEST_SECRET, MockPhaxioService.GetRestClient("accountStatus"));

            Assert.Throws( typeof(ApplicationException), () => phaxio.GetAccountStatus());
        }

        [Test]
        public void UnitTests_InvalidSecretThrowsException()
        {
            var phaxio = new Phaxio(MockPhaxioService.TEST_KEY, "bad_secret", MockPhaxioService.GetRestClient("accountStatus"));

            Assert.Throws(typeof(ApplicationException), () => phaxio.GetAccountStatus());
        }
    }
}
