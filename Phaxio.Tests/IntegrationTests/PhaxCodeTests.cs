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
        public void IntegrationTests_CreatePhaxCodeGetUrl()
        {
            var config = new KeyManager();

            var phaxio = new Phaxio(config["api_key"], config["api_secret"]);

            var code = phaxio.CreatePhaxCode();
        }

        [Test]
        public void IntegrationTests_CreatePhaxCodeGetBytes()
        {
            var config = new KeyManager();

            var phaxio = new Phaxio(config["api_key"], config["api_secret"]);

            var code = phaxio.DownloadPhaxCodePng();

            var hex = BitConverter.ToString(code).Replace("-", "");

            File.WriteAllBytes(@"C:\temp\phaxCode.png", code);
        }
    }
}
