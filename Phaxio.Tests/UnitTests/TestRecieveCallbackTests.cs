using NUnit.Framework;
using Phaxio.Tests.Fixtures;
using Phaxio.Tests.Helpers;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phaxio.Tests.UnitTests
{
    [TestFixture]
    public class TestRecieveCallbackTests
    {
        [Test]
        public void UnitTests_TestRecieveCallback_NoOptions()
        {
            var testPdf = BinaryFixtures.getTestPdfFile();

            Action<IRestRequest> requestAsserts = req =>
            {
                Assert.AreEqual(1, req.Files.Count);
                Assert.AreEqual("filename", req.Files[0].Name);
            };

            var clientBuilder = new IRestClientBuilder { Op = "testReceive", RequestAsserts = requestAsserts };

            var phaxio = new Phaxio(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            var result = phaxio.TestRecieveCallback(testPdf);

            Assert.IsTrue(result.Success, "Result should be Success = true.");
        }

        [Test]
        public void UnitTests_TestRecieveCallback_WithOptions()
        {
            var testPdf = BinaryFixtures.getTestPdfFile();
            var testFromNumber = "8088675309";
            var testToNumber = "2125552368";

            Action<IRestRequest> requestAsserts = req =>
            {
                Assert.AreEqual(1, req.Files.Count);
                Assert.AreEqual("filename", req.Files[0].Name);

                var parameters = ParametersHelper.ToDictionary(req.Parameters);

                Assert.AreEqual(parameters["from_number"], testFromNumber);
                Assert.AreEqual(parameters["to_number"], testToNumber);
            };

            var clientBuilder = new IRestClientBuilder { Op = "testReceive", RequestAsserts = requestAsserts };

            var phaxio = new Phaxio(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            var result = phaxio.TestRecieveCallback(testPdf, fromNumber: testFromNumber, toNumber: testToNumber);

            Assert.IsTrue(result.Success, "Result should be Success = true.");
        }
    }
}
