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
        public void UnitTests_ProvisionNumberWithCallBackRequestWorks()
        {
            var areaCode = "808";

            Action<IRestRequest> requestAsserts = req =>
            {
                Assert.AreEqual(req.Parameters[2].Value, areaCode);
            };

            var clientBuilder = new IRestClientBuilder { Op = "provisionNumber", RequestAsserts = requestAsserts };
            var phaxio = new Phaxio(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            var actualNumber = phaxio.ProvisionNumber(areaCode);

            var expectedNumber = PocoFixtures.GetTestPhoneNumber();

            Assert.AreEqual(expectedNumber.Number, actualNumber.Number, "Number should be the same");
        }

        [Test]
        public void UnitTests_ProvisionNumberRequestWorks()
        {
            var areaCode = "808";
            var callback = "http://example.com/callback";

            Action<IRestRequest> requestAsserts = req =>
            {
                Assert.AreEqual(req.Parameters[2].Value, areaCode);
                Assert.AreEqual(req.Parameters[3].Value, callback);
            };

            var clientBuilder = new IRestClientBuilder { Op = "provisionNumber", RequestAsserts = requestAsserts };
            var phaxio = new Phaxio(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            var actualNumber = phaxio.ProvisionNumber(areaCode, callback);

            var expectedNumber = PocoFixtures.GetTestPhoneNumber();

            Assert.AreEqual(expectedNumber.Number, actualNumber.Number, "Number should be the same");
        }

        [Test]
        public void UnitTests_ListNumbersRequestWorks()
        {
            var areaCode = "808";
            var number = "8088675309";

            Action<IRestRequest> requestAsserts = req =>
            {
                Assert.AreEqual(req.Parameters[2].Value, areaCode);
                Assert.AreEqual(req.Parameters[3].Value, number);
            };

            var clientBuilder = new IRestClientBuilder { Op = "numberList", RequestAsserts = requestAsserts };
            var phaxio = new Phaxio(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            var actualNumbers = phaxio.ListNumbers(areaCode, number);

            var expectedNumbers = PocoFixtures.GetTestPhoneNumbers();

            Assert.AreEqual(expectedNumbers.Count, actualNumbers.Count, "Number should be the same");
        }

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

            var result = phaxio.ReleaseNumber(number);

            Assert.True(result.Success, "Should be success.");
        }
    }
}