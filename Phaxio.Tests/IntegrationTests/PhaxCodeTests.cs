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

            var phaxio = new PhaxioClient(config["api_key"], config["api_secret"]);

            var code = phaxio.CreatePhaxCode();

            Assert.IsNotEmpty(code.AbsoluteUri);
        }

        [Test]
        public void IntegrationTests_PhaxCode_GetCodeBytes()
        {
            var config = new KeyManager();

            var phaxio = new PhaxioClient(config["api_key"], config["api_secret"]);

            var code = phaxio.DownloadPhaxCodePng();

            Assert.IsNotEmpty(code);
        }

        [Test]
        public void IntegrationTests_PhaxCode_AttachCodeAndGetBytes()
        {
            var config = new KeyManager();

            var phaxio = new PhaxioClient(config["api_key"], config["api_secret"]);

            var testPdf = BinaryFixtures.getTestPdfFile();

            var code = phaxio.AttachPhaxCodeToPdf(0, 0, testPdf);

            Assert.IsNotEmpty(code);
        }
    }
}
