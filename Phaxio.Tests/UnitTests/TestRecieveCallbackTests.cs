using NUnit.Framework;
using Phaxio.ThinRestClient;
using Phaxio.Tests.Helpers;
using System;
using Phaxio.Entities.Internal;

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

            var phaxio = new PhaxioClient(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

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

            var phaxio = new PhaxioClient(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            var result = phaxio.TestRecieveCallback(testPdf, fromNumber: testFromNumber, toNumber: testToNumber);

            Assert.IsTrue(result.Success, "Result should be Success = true.");
        }

        [Test]
        public void UnitTests_V2_TestRecieveCallback()
        {
            var testPdf = BinaryFixtures.getTestPdfFile();

            Action<IRestRequest> parameterAsserts = req =>
            {
                Assert.AreEqual(1, req.Files.Count);
                Assert.AreEqual("file", req.Files[0].Name);

                var parameters = ParametersHelper.ToDictionary(req.Parameters);

                Assert.AreEqual((string)parameters["direction"], "received");
                Assert.AreEqual((string)parameters["from_number"], "1");
                Assert.AreEqual((string)parameters["to_number"], "2");
            };

            var requestAsserts = new RequestAsserts()
                .Custom(parameterAsserts)
                .Auth()
                .Post()
                .Resource("faxes")
                .Build();

            var restClient = new RestClientBuilder()
                .WithRequestAsserts(requestAsserts)
                .AsJson()
                .Content(JsonResponseFixtures.FromFile("V2/generic_success"))
                .Ok()
                .Build<Response<Object>>();

            var phaxio = new Phaxio(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, restClient);

            var result = phaxio.TestRecieveCallback(testPdf, fromNumber: "1", toNumber: "2");

            Assert.IsTrue(result.Success, "Result should be Success = true.");
        }
    }
}
