using NUnit.Framework;
using Phaxio.Entities;
using Phaxio.Tests.Helpers;

namespace Phaxio.Tests.UnitTests.V2
{
    [TestFixture]
    public class AccountStatusTests
    {
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

            var phaxio = new PhaxioClient(RestClientBuilder.TEST_KEY, RestClientBuilder.TEST_SECRET, restClient);

            var account = phaxio.Account.Status;

            Assert.AreEqual(15, account.FaxesThisMonth.Sent, "FaxesThisMonth.Sent should be the same.");
            Assert.AreEqual(2, account.FaxesToday.Received, "FaxesToday.Received should be the same.");
            Assert.AreEqual(5050, account.Balance, "Balance should be the same.");
        }
    }
}