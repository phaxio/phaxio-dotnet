using NUnit.Framework;
using Phaxio.ThinRestClient;
using Phaxio.Tests.Helpers;
using System;

namespace Phaxio.Tests.UnitTests.UnitTests.V2
{
    [TestFixture]
    public class TestRecieveCallbackTests
    {
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

            var phaxio = new PhaxioClient(RestClientBuilder.TEST_KEY, RestClientBuilder.TEST_SECRET, restClient);

            var result = phaxio.Fax.TestRecieveCallback(testPdf, fromNumber: "1", toNumber: "2");

            Assert.IsTrue(result.Success, "Result should be Success = true.");
        }
    }
}
