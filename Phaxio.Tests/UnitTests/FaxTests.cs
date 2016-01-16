using System;
using NUnit.Framework;
using Phaxio.Tests.Fixtures;
using RestSharp;

namespace Phaxio.Tests
{
    [TestFixture]
    public class FaxTests
    {
        [Test]
        public void UnitTests_FaxCancelRequestWorks ()
        {
            Action<IRestRequest> requestAsserts = req =>
            {
                Assert.AreEqual(req.Parameters[2].Value, 123);
            };

            var clientBuilder = new IRestClientBuilder { Op = "faxCancel", RequestAsserts = requestAsserts };
            var phaxio = new Phaxio(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            var success = phaxio.CancelFax(123);

            Assert.True(success, "Should be success.");
        }

        [Test]
        public void UnitTests_FaxResendRequestWorks()
        {
            Action<IRestRequest> requestAsserts = req =>
            {
                Assert.AreEqual(req.Parameters[2].Value, 123);
            };

            var clientBuilder = new IRestClientBuilder { Op = "resendFax", RequestAsserts = requestAsserts };
            var phaxio = new Phaxio(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            var success = phaxio.ResendFax(123);

            Assert.True(success, "Should be success.");
        }
    }
}