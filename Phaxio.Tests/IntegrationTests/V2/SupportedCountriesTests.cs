﻿using NUnit.Framework;
using Phaxio.Tests.Helpers;
using Phaxio.V2;
using System.Linq;

namespace Phaxio.Tests.IntegrationTests.V2
{
    [TestFixture, Explicit]
    public class SupportedCountriesTests
    {
        [Test]
        public void IntegrationTests_V2_SupportedCountries()
        {
            var config = new KeyManager();

            var phaxio = new PhaxioV2Client(config["api_key"], config["api_secret"]);

            var countries = phaxio.ListSupportedCountries();

            Assert.Greater(countries.Data.Count(), 0, "There should be some countries");
        }
    }
}