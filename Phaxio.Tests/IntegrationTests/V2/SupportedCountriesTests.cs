using NUnit.Framework;
using Phaxio.Tests.Helpers;
using System.Linq;

namespace Phaxio.Tests.IntegrationTests.V2
{
    [TestFixture, Explicit]
    public class SupportedCountriesTests
    {
        [Test]
        public void IntegrationTests_V2_Public_SupportedCountry_List()
        {
            var config = new KeyManager();

            var phaxio = new PhaxioContext(config["api_key"], config["api_secret"]);

            var countries = phaxio.Public.SupportedCountry.List();

            Assert.Greater(countries.Count(), 0, "There should be some countries");
        }
    }
}
