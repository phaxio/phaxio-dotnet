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
        public void IntegrationTests_V2_Fax_Create()
        {
            var config = new KeyManager();

            var phaxio = new PhaxioContext(config["api_key"], config["api_secret"]);

            var testPdf = BinaryFixtures.getTestPdfFile();

            var fax = phaxio.Fax.Create(to: "+18088675309", file: testPdf);

            Assert.NotZero(fax.Id);
        }

        [Test]
        // This is test runs faily long since Phaxio rate limits
        public void IntegrationTests_V2_Fax_BasicScenario()
        {
            var config = new KeyManager();

            var phaxio = new PhaxioContext(config["api_key"], config["api_secret"]);

            var phaxioV1 = new PhaxioClient(config["api_key"], config["api_secret"]);

            // Create a phax code
            var metadata = StringHelpers.Random(10);

            var phaxCode = phaxio.PhaxCode.Create(metadata);

            var phaxCodeFilename = pwd() + metadata + ".png";

            filesToCleanup.Add(phaxCodeFilename);

            File.WriteAllBytes(phaxCodeFilename, phaxCode.Png);

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
            var fax = phaxio.Fax.Create(to: "8088675309", file: testPdfWithCode);

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
                    var thumbnailBytes = phaxio.Fax.Retrieve(fax.Id).File.SmallJpeg.Bytes;
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
            fax.Resend();

            Thread.Sleep(1000);

            // Cancel a fax
            Assert.Throws<Exception>(() => fax.Cancel(), "CancelFax should throw exception.");

            Thread.Sleep(1000);

            // Delete a fax
            fax.File.Delete();

            Thread.Sleep(1000);

            // Delete a fax
            fax.Delete();
        }
    }
}
