using NUnit.Framework;
using Phaxio.Tests.Helpers;
using System.Threading;

namespace Phaxio.Tests.IntegrationTests.V2
{
    [TestFixture, Explicit]
    public class PhaxCodeTests
    {
        [Test]
        public void IntegrationTests_V2_PhaxCodeRepository_Create()
        {
            var config = new KeyManager();

            var phaxio = new PhaxioClient(config["api_key"], config["api_secret"]);

            var phaxCode = phaxio.PhaxCode.Create("stuff");

            Assert.IsNotEmpty(phaxCode.Identifier);
        }

        [Test]
        public void IntegrationTests_V2_PhaxCode_Png()
        {
            var config = new KeyManager();

            var phaxio = new PhaxioClient(config["api_key"], config["api_secret"]);

            var png = phaxio.PhaxCode.Create("metadata").Png;

            Assert.IsNotEmpty(png);
        }

        [Test]
        public void IntegrationTests_V2_PhaxCode_BasicScenario()
        {
            var config = new KeyManager();

            var phaxio = new PhaxioClient(config["api_key"], config["api_secret"]);

            var code1 = phaxio.PhaxCode.Create("phil");

            Thread.Sleep(1000);

            var code2 = phaxio.PhaxCode.Retrieve(code1.Identifier);

            Assert.AreEqual("phil", code2.Metadata);

            Thread.Sleep(1000);

            Assert.IsNotEmpty(code2.Png);
        }
    }
}
