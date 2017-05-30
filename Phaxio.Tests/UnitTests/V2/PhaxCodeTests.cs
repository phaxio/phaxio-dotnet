using NUnit.Framework;
using Phaxio.Resources.V2;
using Phaxio.Tests.Helpers;
using Phaxio.ThinRestClient;
using System;

namespace Phaxio.Tests.UnitTests.UnitTests.V2
{
    [TestFixture]
    public class PhaxCodeTests
    {
        [Test]
        public void UnitTests_V2_PhaxCode_Create()
        {
            Action<IRestRequest> parameterAsserts = req =>
            {
                var parameters = ParametersHelper.ToDictionary(req.Parameters);

                Assert.AreEqual("stuff", parameters["metadata"]);
            };

            var requestAsserts = new RequestAsserts()
                .Auth()
                .Post()
                .Custom(parameterAsserts)
                .Resource("phax_codes.json")
                .Build();

            var restClient = new RestClientBuilder()
                .WithRequestAsserts(requestAsserts)
                .AsJson()
                .Content(JsonResponseFixtures.FromFile("V2/phax_code"))
                .Ok()
                .Build<Response<PhaxCode>>();

            var phaxio = new PhaxioContext(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, restClient);

            var code = phaxio.PhaxCode.Create("stuff");

            Assert.AreEqual("1234", code.Identifier);
        }

        [Test]
        public void UnitTests_V2_PhaxCode_Retrieve()
        {
            var requestAsserts = new RequestAsserts()
                .Auth()
                .Get()
                .Resource("phax_code.json")
                .Build();

            var restClient = new RestClientBuilder()
                .WithRequestAsserts(requestAsserts)
                .AsJson()
                .Content(JsonResponseFixtures.FromFile("V2/phax_code"))
                .Ok()
                .Build<Response<PhaxCode>>();

            var phaxio = new PhaxioContext(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, restClient);

            var code = phaxio.PhaxCode.Retrieve();

            Assert.AreEqual("1234", code.Identifier);
        }

        [Test]
        public void UnitTests_V2_PhaxCode_RetrieveWithId()
        {
            var requestAsserts = new RequestAsserts()
                .Auth()
                .Get()
                .Resource("phax_codes/1234.json")
                .Build();

            var restClient = new RestClientBuilder()
                .WithRequestAsserts(requestAsserts)
                .AsJson()
                .Content(JsonResponseFixtures.FromFile("V2/phax_code"))
                .Ok()
                .Build<Response<PhaxCode>>();

            var phaxio = new PhaxioContext(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, restClient);

            var code = phaxio.PhaxCode.Retrieve("1234");

            Assert.AreEqual("1234", code.Identifier);
        }

        [Test]
        public void UnitTests_V2_PhaxCode_RetrievePngWithIdentifier()
        {
            var pngBytes = BinaryFixtures.GetTestPhaxCode();

            var requestAsserts = new RequestAsserts()
                .Auth()
                .Get()
                .Resource("phax_codes/1234.png")
                .Build();

            var restClient = new RestClientBuilder()
                .WithRequestAsserts(requestAsserts)
                .AsPng()
                .RawBytes(pngBytes)
                .Ok()
                .Build();

            var phaxio = new PhaxioContext(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, restClient);

            var phaxCode = new PhaxCode { Identifier = "1234", PhaxioClient = phaxio };

            Assert.AreEqual(pngBytes, phaxCode.Png);
        }
    }
}