using NUnit.Framework;
using Phaxio.Entities;
using Phaxio.Tests.Helpers;
using System.Threading;

namespace Phaxio.Tests.IntegrationTests.V2
{
    [TestFixture, Explicit]
    public class FaxQueryTests
    {
        [Test]
        public void IntegrationTests_V2_Fax_List()
        {
            var config = new KeyManager();

            var phaxio = new Phaxio(config["api_key"], config["api_secret"]);

            var testPdf = BinaryFixtures.getTestPdfFile();

            var request = new FaxRequest { ToNumber = "+18088675309", File = testPdf };

            var faxId = phaxio.SendFax(request);

            Assert.IsNotEmpty(faxId);

            Thread.Sleep(1000);

            var faxes = phaxio.ListFaxes();

            Assert.Greater(faxes.Data.Count, 0, "There should be some faxes.");
        }
    }
}
