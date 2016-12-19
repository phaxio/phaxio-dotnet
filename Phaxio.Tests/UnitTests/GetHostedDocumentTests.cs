using System;
using NUnit.Framework;
using Phaxio.ThinRestClient;
using Phaxio.Tests.Helpers;

namespace Phaxio.Tests
{
    [TestFixture]
    public class GetHostedDocumentTests
    {
        [Test]
        public void UnitTests_GetHostedDocument()
        {
            var filename = "hostedoc.pdf";

            Action<IRestRequest> requestAsserts = req =>
            {
                var parameters = ParametersHelper.ToDictionary(req.Parameters);

                Assert.AreEqual(parameters["name"], filename);
            };

            var clientBuilder = new IRestClientBuilder { Op = "getHostedDocument", RequestAsserts = requestAsserts };

            var phaxio = new PhaxioClient(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.BuildUntyped());

            var testPdf = BinaryFixtures.getTestPdfFile();

            var pdfBytes = phaxio.GetHostedDocument(filename);

            Assert.IsNotEmpty(pdfBytes);

            var expectedPdf = BinaryFixtures.GetTestPdf();

            Assert.AreEqual(expectedPdf, pdfBytes, "PDFs should be the same.");
        }

        [Test]
        public void UnitTests_GetHostedDocument_WithMetadata()
        {
            var metadata = "key";
            var filename = "hostedoc.pdf";

            Action<IRestRequest> requestAsserts = req =>
            {
                var parameters = ParametersHelper.ToDictionary(req.Parameters);

                Assert.AreEqual(parameters["name"], filename);
                Assert.AreEqual(parameters["metadata"], metadata);
            };

            var clientBuilder = new IRestClientBuilder { Op = "getHostedDocument", RequestAsserts = requestAsserts };

            var phaxio = new PhaxioClient(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.BuildUntyped());

            var testPdf = BinaryFixtures.getTestPdfFile();

            var pdfBytes = phaxio.GetHostedDocument(filename, metadata: metadata);

            Assert.IsNotEmpty(pdfBytes);

            var expectedPdf = BinaryFixtures.GetTestPdf();

            Assert.AreEqual(expectedPdf, pdfBytes, "PDFs should be the same.");
        }
    }
}