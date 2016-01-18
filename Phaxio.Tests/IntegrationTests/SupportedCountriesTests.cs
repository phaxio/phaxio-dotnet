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
    public class SupportedCountriesTests
    {
        [Test]
        public void IntegrationTests_SupportedCountries()
        {
            var config = new KeyManager();

            var phaxio = new Phaxio(config["api_key"], config["api_secret"]);

            var countries = phaxio.ListSupportedCountries();

            Assert.Greater(countries.Count(), 0, "There should be some countries");
        }
    }
}
