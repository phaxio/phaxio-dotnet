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

namespace Phaxio.Tests.IntegrationTests
{
    [TestFixture, Explicit]
    public class PhaxCodeTests
    {
        [Test]
        public void IntegrationTests_PhaxCode_GetCodeUrl()
        {
            var config = new KeyManager();

            var phaxio = new Phaxio(config["api_key"], config["api_secret"]);

            var code = phaxio.CreatePhaxCode();

            Assert.IsNotEmpty(code.Address.AbsoluteUri);
        }

        [Test]
        public void IntegrationTests_PhaxCode_GetCodeBytes()
        {
            var config = new KeyManager();

            var phaxio = new Phaxio(config["api_key"], config["api_secret"]);

            var code = phaxio.DownloadPhaxCodePng();

            Assert.IsNotEmpty(code);
        }

        [Test]
        public void IntegrationTests_PhaxCode_AttachCodeAndGetBytes()
        {
            var config = new KeyManager();

            var phaxio = new Phaxio(config["api_key"], config["api_secret"]);

            var testPdf = BinaryFixtures.getTestPdfFile();

            var code = phaxio.AttachPhaxCodeToPdf(0, 0, testPdf);

            Assert.IsNotEmpty(code);
        }

        [Test]
        public void IntegrationTests__PhaxCode_AttachAndStreamRequestWorks()
        {
            var config = new KeyManager();

            var phaxio = new Phaxio(config["api_key"], config["api_secret"]);

            var testPdf = BinaryFixtures.getTestPdfFile();

            var memoryStream = new MemoryStream();

            phaxio.AttachPhaxCodeToPdf(0, 0, testPdf, memoryStream);

            var expectedPdf = BinaryFixtures.GetTestPdf();

            var bytes = memoryStream.ToArray();

            Assert.IsNotEmpty(bytes);
        }
    }
}
