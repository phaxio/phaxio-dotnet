using NUnit.Framework;
using Phaxio.Tests.Fixtures;
using Phaxio.Tests.Helpers;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Phaxio.Tests.IntegrationTests
{
    [TestFixture, Explicit]
    public class FaxTests
    {
        private List<string> filesToCleanup = new List<string>();

        [TearDown]
        public void Teardown()
        {
            foreach(var file in filesToCleanup)
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
        }

        [Test]
        public void IntegrationTests_Fax_Send()
        {
            var config = new KeyManager();

            var phaxio = new PhaxioClient(config["api_key"], config["api_secret"]);

            var testPdf = BinaryFixtures.getTestPdfFile();

            var faxId = phaxio.SendFax("8088675309", testPdf);

            Assert.IsNotEmpty(faxId);
        }

        [Test]
        // This is test runs faily long since Phaxio rate limits
        public void IntegrationTests_Fax_BasicScenario()
        {
            var config = new KeyManager();

            var phaxio = new PhaxioClient(config["api_key"], config["api_secret"]);

            // Create a phax code
            var metadata = StringHelpers.Random(10);

            var phaxCodePng = phaxio.DownloadPhaxCodePng(metadata);

            var phaxCodeFilename = metadata + ".png";

            filesToCleanup.Add(phaxCodeFilename);

            File.WriteAllBytes(phaxCodeFilename, phaxCodePng);

            // Attach phax code to pdf
            var testPdf = BinaryFixtures.getTestPdfFile();

            var testPdfWithCodeBytes = phaxio.AttachPhaxCodeToPdf(0, 0, testPdf, metadata: metadata);

            var testPdfWithCodeFilename = metadata + ".pdf";

            filesToCleanup.Add(testPdfWithCodeFilename);

            File.WriteAllBytes(testPdfWithCodeFilename, testPdfWithCodeBytes);

            var testPdfWithCode = new FileInfo(testPdfWithCodeFilename);

            // Send phax using pdf with phax code
            var faxId = phaxio.SendFax("8088675309", testPdfWithCode);

            // Phaxio rate limits, so we need to wait a second.
            Thread.Sleep(100);

            // Download a thumbnail of the sent fax
            // It takes a little while for a fax to show up
            int retries = 0;
            bool downloadSuccess = false;
            while (retries < 20 && !downloadSuccess)
            {
                try
                {
                    var thumbnailBytes = phaxio.DownloadFax(faxId, "s");
                    var thumbnailFilename = metadata + ".jpg";

                    filesToCleanup.Add(thumbnailFilename);

                    File.WriteAllBytes(thumbnailFilename, thumbnailBytes);

                    downloadSuccess = true;
                }
                catch (Exception)
                {
                    retries++;
                    Thread.Sleep(1000);
                }
            }

            Assert.IsTrue(downloadSuccess, "DownloadFax should've worked");

            // Resend fax
            var resendResult = phaxio.ResendFax(faxId);

            Assert.True(resendResult.Success, "ResendFax should return success.");

            Thread.Sleep(500);

            // Cancel a fax
            var cancelResult = phaxio.CancelFax(faxId);

            Assert.IsFalse(cancelResult.Success, "CancelFax should not be successful.");

            Thread.Sleep(500);

            // Delete a fax
            var deleteResult = phaxio.DeleteFax(faxId);

            Assert.True(resendResult.Success, "DeleteResult should return success.");
        }
    }
}
