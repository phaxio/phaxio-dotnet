using NUnit.Framework;
using Phaxio.Tests.Fixtures;
using Phaxio.Tests.Helpers;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phaxio.Tests.IntegrationTests
{
    [TestFixture, Explicit]
    public class FaxTests
    {
        [Test]
        public void IntegrationTests_Fax_Send()
        {
            var config = new KeyManager();

            var phaxio = new Phaxio(config["api_key"], config["api_secret"]);

            var testPdf = BinaryFixtures.getTestPdfFile();

            var faxId = phaxio.Send("8088675309", testPdf);

            Assert.IsNotEmpty(faxId);
        }
    }
}
