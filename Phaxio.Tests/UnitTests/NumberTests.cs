using System;
using NUnit.Framework;
using Phaxio.Tests.Fixtures;
using RestSharp;

namespace Phaxio.Tests
{
    [TestFixture]
    public class NumberTests
    {
        [Test]
        public void UnitTests_ReleaseNumberRequestWorks ()
        {
            var number = "8088675309";

            Action<IRestRequest> requestAsserts = req =>
            {
                Assert.AreEqual(req.Parameters[2].Value, number);
            };

            var clientBuilder = new IRestClientBuilder { Op = "releaseNumber", RequestAsserts = requestAsserts };
            var phaxio = new Phaxio(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            var success = phaxio.ReleaseNumber(number);

            Assert.True(success, "Should be success.");
        }
    }
}