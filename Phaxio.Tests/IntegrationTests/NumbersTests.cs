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
    public class NumbersTests
    {
        [Test]
        public void IntegrationTests_Numbers_GetAreaCodes()
        {
            var config = new KeyManager();

            var phaxio = new Phaxio(config["api_key"], config["api_secret"]);

            var areaCodes = phaxio.GetAreaCodes(state:"HI");

            Assert.Greater(areaCodes.Count(), 0, "There should be some area codes");
        }
    }
}
