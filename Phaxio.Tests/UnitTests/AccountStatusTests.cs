using System;
using NUnit.Framework;

namespace Phaxio.Tests
{
    [TestFixture]
    public class AccountStatusTests
    {
        [Test]
        public void UnitTest_AccountStatusRequestWorks ()
        {
            var phaxio = new Phaxio(MockPhaxioService.TEST_KEY, MockPhaxioService.TEST_SECRET, MockPhaxioService.GetRestClient("accountStatus"));

            var account = phaxio.GetAccountStatus();

            var expectedAccount = MockPhaxioService.GetTestAccount();

            Assert.AreEqual(expectedAccount.FaxesSentThisMonth, account.FaxesSentThisMonth, "FaxesSentThisMonth should be the same.");
            Assert.AreEqual(expectedAccount.FaxesSentToday, account.FaxesSentToday, "FaxesSentThisWeek should be the same.");
            Assert.AreEqual(expectedAccount.Balance, account.Balance, "Balance should be the same.");
        }
    }
}