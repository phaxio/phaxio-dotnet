using NUnit.Framework;
using Phaxio.Tests.Helpers;
using System.Linq;

namespace Phaxio.Tests.IntegrationTests
{
    [TestFixture, Explicit]
    public class SupportedCountriesTests
    {
        [Test]
        public void IntegrationTests_SupportedCountries()
        {
            var config = new KeyManager();

            var phaxio = new PhaxioClient(config["api_key"], config["api_secret"]);

            var countries = phaxio.ListSupportedCountries();

            Assert.Greater(countries.Count(), 0, "There should be some countries");
        }
    }
}
