using NUnit.Framework;
using Phaxio.Tests.Helpers;
using System;

namespace Phaxio.Tests.IntegrationTests
{
    [TestFixture, Explicit]
    class GetHostedDocumentTests
    {
        [Test]
        public void IntegrationTests_GetHostedDocument()
        {
            var config = new KeyManager();

            var phaxio = new PhaxioClient(config["api_key"], config["api_secret"]);

            var filename = "hostedoc.pdf";

            Assert.Throws(typeof(ApplicationException), () => phaxio.GetHostedDocument(filename));
        }
    }
}
