using NUnit.Framework;
using Phaxio.Tests.Fixtures;

namespace Phaxio.Tests
{
    [TestFixture]
    public class AccountStatusTests
    {
        [Test]
        public void UnitTests_AccountStatus ()
        {
            var clientBuilder = new IRestClientBuilder { Op = "accountStatus" };
            var phaxio = new PhaxioClient(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            var account = phaxio.GetAccountStatus();

            var expectedAccount = PocoFixtures.GetTestAccount();

            Assert.AreEqual(expectedAccount.FaxesSentThisMonth, account.FaxesSentThisMonth, "FaxesSentThisMonth should be the same.");
            Assert.AreEqual(expectedAccount.FaxesSentToday, account.FaxesSentToday, "FaxesSentThisWeek should be the same.");
            Assert.AreEqual(expectedAccount.Balance, account.Balance, "Balance should be the same.");
        }
    }
}