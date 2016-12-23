using NUnit.Framework;
using Phaxio.Tests.Helpers;
using Phaxio.V2;
using System.Threading;

namespace Phaxio.Tests.IntegrationTests.V2
{
    [TestFixture, Explicit]
    public class PhaxCodeTests
    {
        [Test]
        public void IntegrationTests_V2_PhaxCode_GetId()
        {
            var config = new KeyManager();

            var phaxio = new PhaxioV2Client(config["api_key"], config["api_secret"]);

            var codeId = phaxio.GeneratePhaxCode("stuff");

            Assert.IsNotEmpty(codeId);
        }

        [Test]
        public void IntegrationTests_V2_PhaxCode_GetCodeBytes()
        {
            var config = new KeyManager();

            var phaxio = new PhaxioV2Client(config["api_key"], config["api_secret"]);

            var png = phaxio.GeneratePhaxCodeAndDownload();

            Assert.IsNotEmpty(png);
        }

        [Test]
        public void IntegrationTests_V2_PhaxCode_BasicScenario()
        {
            var config = new KeyManager();

            var phaxio = new PhaxioV2Client(config["api_key"], config["api_secret"]);

            var codeId = phaxio.GeneratePhaxCode("phil");

            Thread.Sleep(1000);

            var properties = phaxio.GetPhaxCode(codeId);

            Assert.AreEqual("phil", properties.Metadata);

            Thread.Sleep(1000);

            var png = phaxio.DownloadPhaxCode(codeId);

            Assert.IsNotEmpty(png);
        }
    }
}
