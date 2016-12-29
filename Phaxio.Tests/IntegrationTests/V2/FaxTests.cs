using NUnit.Framework;
using Phaxio.Entities;
using Phaxio.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Phaxio.Tests.IntegrationTests.V2
{
    [TestFixture, Explicit]
    public class FaxTests
    {
        private List<string> filesToCleanup = new List<string>();

        private string pwd()
        {
            return AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        }

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
        public void IntegrationTests_V2_Fax_Send()
        {
            var config = new KeyManager();

            var phaxio = new Phaxio(config["api_key"], config["api_secret"]);

            var testPdf = BinaryFixtures.getTestPdfFile();

            var request = new FaxRequest { ToNumber = "+18088675309", File = testPdf };

            var faxId = phaxio.SendFax(request);

            Assert.IsNotEmpty(faxId);
        }

        [Test]
        // This is test runs faily long since Phaxio rate limits
        public void IntegrationTests_V2_Fax_BasicScenario()
        {
            var config = new KeyManager();

            var phaxio = new Phaxio(config["api_key"], config["api_secret"]);

            var phaxioV1 = new PhaxioClient(config["api_key"], config["api_secret"]);

            // Create a phax code
            var metadata = StringHelpers.Random(10);

            var phaxCodePng = phaxio.GeneratePhaxCodeAndDownload(metadata);

            var phaxCodeFilename = pwd() + metadata + ".png";

            filesToCleanup.Add(phaxCodeFilename);

            File.WriteAllBytes(phaxCodeFilename, phaxCodePng);

            // Attach phax code to pdf
            var testPdf = BinaryFixtures.getTestPdfFile();

            var testPdfWithCodeBytes = phaxioV1.AttachPhaxCodeToPdf(0, 0, testPdf, metadata: metadata);

            var testPdfWithCodeFilename = pwd() + metadata + ".pdf";

            filesToCleanup.Add(testPdfWithCodeFilename);

            File.WriteAllBytes(testPdfWithCodeFilename, testPdfWithCodeBytes);

            var testPdfWithCode = new FileInfo(testPdfWithCodeFilename);

            // Phaxio rate limits, so we need to wait a second.
            Thread.Sleep(1000);

            // Send phax using pdf with phax code
            var faxRequest = new FaxRequest { ToNumber = "8088675309", File = testPdfWithCode };
            var faxId = phaxio.SendFax(faxRequest);

            // Phaxio rate limits, so we need to wait a second.
            Thread.Sleep(1000);

            // Download a thumbnail of the sent fax
            // It takes a little while for a fax to show up
            int retries = 0;
            bool downloadSuccess = false;
            while (retries < 20 && !downloadSuccess)
            {
                try
                {
                    var thumbnailBytes = phaxio.DownloadFax(faxId, "s");
                    var thumbnailFilename = pwd() + metadata + ".jpg";

                    filesToCleanup.Add(thumbnailFilename);

                    File.WriteAllBytes(thumbnailFilename, thumbnailBytes);

                    downloadSuccess = true;
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                    retries++;
                    Thread.Sleep(1000);
                }
            }

            Assert.IsTrue(downloadSuccess, "DownloadFax should've worked");

            Thread.Sleep(2000);

            // Resend fax
            var resendResult = phaxio.ResendFax(faxId);

            Assert.True(resendResult.Success, "ResendFax should return success.");

            Thread.Sleep(1000);

            // Cancel a fax
            var cancelResult = phaxio.CancelFax(faxId);

            Assert.IsFalse(cancelResult.Success, "CancelFax should not be successful.");

            Thread.Sleep(1000);

            // Delete a fax
            var deleteFileResult = phaxio.DeleteFaxFiles(faxId);

            Assert.True(deleteFileResult.Success, "DeleteFileResult should return success.");

            Thread.Sleep(1000);

            // Delete a fax
            var deleteResult = phaxio.DeleteFax(faxId);

            Assert.True(deleteResult.Success, "DeleteResult should return success.");
        }
    }
}
