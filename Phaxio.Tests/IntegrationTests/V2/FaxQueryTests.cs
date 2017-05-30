using NUnit.Framework;
using Phaxio.Entities;
using Phaxio.Tests.Helpers;
using System.Threading;
using System.Linq;

namespace Phaxio.Tests.IntegrationTests.V2
{
    [TestFixture, Explicit]
    public class FaxQueryTests
    {
        [Test]
        public void IntegrationTests_V2_Fax_List()
        {
            var config = new KeyManager();

            var phaxio = new PhaxioContext(config["api_key"], config["api_secret"]);

            var testPdf = BinaryFixtures.getTestPdfFile();

            var fax = phaxio.Fax.Create(to: "+18088675309", file: testPdf);

            Assert.NotZero(fax.Id);

            Thread.Sleep(1000);

            var faxes = phaxio.Fax.List();

            Assert.Greater(faxes.Count(), 0, "There should be some faxes.");
        }
    }
}
