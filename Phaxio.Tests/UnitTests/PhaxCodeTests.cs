using NUnit.Framework;
using Phaxio.Entities;
using Phaxio.Entities.Internal;
using Phaxio.Tests.Fixtures;
using Phaxio.Tests.Helpers;
using Phaxio.ThinRestClient;
using Phaxio.V2;
using System;

namespace Phaxio.Tests.UnitTests
{
    [TestFixture]
    public class PhaxCodeTests
    {
        [Test]
        public void UnitTests_PhaxCode_CreateWithUrl_NoOption()
        {
            var clientBuilder = new IRestClientBuilder { Op = "createPhaxCodeUrl" };

            var phaxio = new PhaxioClient(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            var phaxCodeUrl = phaxio.CreatePhaxCode();

            var expectedPhaxCodeUrl = PocoFixtures.GetTestPhaxCodeUrl();

            Assert.AreEqual(expectedPhaxCodeUrl, phaxCodeUrl, "URLs should be the same.");
        }

        [Test]
        public void UnitTests_PhaxCode_CreateWithUrlAndOptions()
        {
            var metadata = "key=value";

            Action<IRestRequest> requestAsserts = req =>
            {
                var parameters = ParametersHelper.ToDictionary(req.Parameters);
                Assert.AreEqual(metadata, parameters["metadata"], "y's should be the same.");
            };

            var clientBuilder = new IRestClientBuilder { Op = "createPhaxCodeUrl", RequestAsserts = requestAsserts };

            var phaxio = new PhaxioClient(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            var phaxCodeUrl = phaxio.CreatePhaxCode(metadata);

            var expectedPhaxCodeUrl = PocoFixtures.GetTestPhaxCodeUrl();

            Assert.AreEqual(expectedPhaxCodeUrl, phaxCodeUrl, "URLs should be the same.");
        }

        [Test]
        public void UnitTests_PhaxCode_DownloadPng()
        {
            var clientBuilder = new IRestClientBuilder { Op = "createPhaxCodeDownload" };

            var phaxio = new PhaxioClient(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.BuildUntyped());

            var imageBytes = phaxio.DownloadPhaxCodePng();

            var expectedImageBytes = BinaryFixtures.GetTestPhaxCode();

            Assert.AreEqual(expectedImageBytes, imageBytes, "Images should be the same.");
        }

        [Test]
        public void UnitTests_PhaxCode_BadDownloadGetsErrorMessage()
        {
            var clientBuilder = new IRestClientBuilder { Op = "createPhaxCodeDownload" };

            var phaxio = new PhaxioClient(IRestClientBuilder.TEST_KEY + "bad stuff", IRestClientBuilder.TEST_SECRET, clientBuilder.BuildUntyped());

            var exception = Assert.Throws(typeof(ApplicationException), () => phaxio.DownloadPhaxCodePng());

            Assert.AreEqual("That key or secret is not correct.", exception.Message, "Exception message should be about the auth failure.");
        }

        [Test]
        public void UnitTests_PhaxCode_AttachNoOptions()
        {
            var testPdf = BinaryFixtures.getTestPdfFile();

            Action<IRestRequest> requestAsserts = req =>
            {
                Assert.AreEqual(1, req.Files.Count);
                Assert.AreEqual("filename", req.Files[0].Name);

                var parameters = ParametersHelper.ToDictionary(req.Parameters);

                Assert.AreEqual(1, parameters["x"], "x's should be the same.");
                Assert.AreEqual(2, parameters["y"], "y's should be the same.");
            };

            var clientBuilder = new IRestClientBuilder { Op = "attachPhaxCodeToPdf", RequestAsserts = requestAsserts };

            var phaxio = new PhaxioClient(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.BuildUntyped());

            var pdfBytes = phaxio.AttachPhaxCodeToPdf(1, 2, testPdf);

            Assert.IsNotEmpty(pdfBytes);

            var expectedPdf = BinaryFixtures.GetTestPdf();

            Assert.AreEqual(expectedPdf, pdfBytes, "PDFs should be the same.");
        }

        [Test]
        public void UnitTests_PhaxCode_AttachWithOptions()
        {
            var metadata = "key=value";
            var testPdf = BinaryFixtures.getTestPdfFile();

            Action<IRestRequest> requestAsserts = req =>
            {
                Assert.AreEqual(1, req.Files.Count);
                Assert.AreEqual("filename", req.Files[0].Name);

                var parameters = ParametersHelper.ToDictionary(req.Parameters);

                Assert.AreEqual(1, parameters["x"], "x's should be the same.");
                Assert.AreEqual(2, parameters["y"], "y's should be the same.");
                Assert.AreEqual(metadata, parameters["metadata"], "y's should be the same.");
                Assert.AreEqual(3, parameters["page_number"], "y's should be the same.");
            };

            var clientBuilder = new IRestClientBuilder { Op = "attachPhaxCodeToPdf", RequestAsserts = requestAsserts };

            var phaxio = new PhaxioClient(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.BuildUntyped());

            var pdfBytes = phaxio.AttachPhaxCodeToPdf(1, 2, testPdf, metadata: metadata, pageNumber:3);

            Assert.IsNotEmpty(pdfBytes);

            var expectedPdf = BinaryFixtures.GetTestPdf();

            Assert.AreEqual(expectedPdf, pdfBytes, "PDFs should be the same.");
        }

        [Test]
        public void UnitTests_V2_PhaxCode_Generate()
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
                .Build<Response<dynamic>>();

            var phaxio = new PhaxioV2Client(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, restClient);

            var identifier = phaxio.GeneratePhaxCode("stuff");

            Assert.AreEqual("1234", identifier);
        }

        [Test]
        public void UnitTests_V2_PhaxCode_GeneratePng()
        {
            var pngBytes = BinaryFixtures.GetTestPhaxCode();

            Action<IRestRequest> parameterAsserts = req =>
            {
                var parameters = ParametersHelper.ToDictionary(req.Parameters);

                Assert.AreEqual("stuff", parameters["metadata"]);
            };

            var requestAsserts = new RequestAsserts()
                .Auth()
                .Post()
                .Custom(parameterAsserts)
                .Resource("phax_codes.png")
                .Build();

            var restClient = new RestClientBuilder()
                .WithRequestAsserts(requestAsserts)
                .AsPng()
                .RawBytes(pngBytes)
                .Ok()
                .Build();

            var phaxio = new PhaxioV2Client(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, restClient);

            var image = phaxio.GeneratePhaxCodeAndDownload("stuff");

            Assert.AreEqual(pngBytes, image);
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

            var phaxio = new PhaxioV2Client(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, restClient);

            var code = phaxio.GetPhaxCode();

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

            var phaxio = new PhaxioV2Client(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, restClient);

            var code = phaxio.GetPhaxCode("1234");

            Assert.AreEqual("1234", code.Identifier);
        }

        [Test]
        public void UnitTests_V2_PhaxCode_RetrievePng()
        {
            var pngBytes = BinaryFixtures.GetTestPhaxCode();

            var requestAsserts = new RequestAsserts()
                .Auth()
                .Get()
                .Resource("phax_code.png")
                .Build();

            var restClient = new RestClientBuilder()
                .WithRequestAsserts(requestAsserts)
                .AsPng()
                .RawBytes(pngBytes)
                .Ok()
                .Build();

            var phaxio = new PhaxioV2Client(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, restClient);

            var image = phaxio.DownloadPhaxCode();

            Assert.AreEqual(pngBytes, image);
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

            var phaxio = new PhaxioV2Client(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, restClient);

            var image = phaxio.DownloadPhaxCode("1234");

            Assert.AreEqual(pngBytes, image);
        }
    }
}