using NUnit.Framework;
using Phaxio.Entities;
using Phaxio.Entities.Internal;
using Phaxio.Tests.Fixtures;
using Phaxio.Tests.Helpers;

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

        [Test]
        public void UnitTests_V2_AccountStatus()
        {
            var requestAsserts = new RequestAsserts()
                .Auth()
                .Get()
                .Resource("account/status")
                .Build();

            var restClient = new RestClientBuilder()
                .WithRequestAsserts(requestAsserts)
                .AsJson()
                .Content(JsonResponseFixtures.FromFile("V2/account_status"))
                .Ok()
                .Build<Response<AccountStatus>>();

            var phaxio = new Phaxio(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, restClient);

            var account = phaxio.GetAccountStatus();

            Assert.AreEqual(15, account.FaxesThisMonth.Sent, "FaxesThisMonth.Sent should be the same.");
            Assert.AreEqual(2, account.FaxesToday.Received, "FaxesToday.Received should be the same.");
            Assert.AreEqual(5050, account.Balance, "Balance should be the same.");
        }
    }
}