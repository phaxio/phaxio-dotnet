using NUnit.Framework;
using Phaxio.Tests.Fixtures;
using Phaxio.Tests.Helpers;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phaxio.Tests.UnitTests
{
    [TestFixture]
    public class PhaxCodeTests
    {
        [Test]
        public void UnitTests_PhaxCode_CreateWithUrl_NoOption()
        {
            var clientBuilder = new IRestClientBuilder { Op = "createPhaxCodeUrl" };

            var phaxio = new Phaxio(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

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

            var phaxio = new Phaxio(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            var phaxCodeUrl = phaxio.CreatePhaxCode(metadata);

            var expectedPhaxCodeUrl = PocoFixtures.GetTestPhaxCodeUrl();

            Assert.AreEqual(expectedPhaxCodeUrl, phaxCodeUrl, "URLs should be the same.");
        }

        [Test]
        public void UnitTests_PhaxCode_DownloadPng()
        {
            var clientBuilder = new IRestClientBuilder { Op = "createPhaxCodeDownload" };

            var phaxio = new Phaxio(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.BuildUntyped());

            var imageBytes = phaxio.DownloadPhaxCodePng();

            var expectedImageBytes = BinaryFixtures.GetTestPhaxCode();

            Assert.AreEqual(expectedImageBytes, imageBytes, "Images should be the same.");
        }

        [Test]
        public void UnitTests_PhaxCode_BadDownloadGetsErrorMessage()
        {
            var clientBuilder = new IRestClientBuilder { Op = "createPhaxCodeDownload" };

            var phaxio = new Phaxio(IRestClientBuilder.TEST_KEY + "bad stuff", IRestClientBuilder.TEST_SECRET, clientBuilder.BuildUntyped());

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

            var phaxio = new Phaxio(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.BuildUntyped());

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

            var phaxio = new Phaxio(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.BuildUntyped());

            var pdfBytes = phaxio.AttachPhaxCodeToPdf(1, 2, testPdf, metadata: metadata, pageNumber:3);

            Assert.IsNotEmpty(pdfBytes);

            var expectedPdf = BinaryFixtures.GetTestPdf();

            Assert.AreEqual(expectedPdf, pdfBytes, "PDFs should be the same.");
        }
    }
}