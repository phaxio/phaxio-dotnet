using NUnit.Framework;
using Phaxio.Tests.Fixtures;
using System.Linq;

namespace Phaxio.Tests.UnitTests
{
    [TestFixture]
    public class SupportedCountriesTests
    {
        [Test]
        public void UnitTests_SupportedCountries()
        {
            var clientBuilder = new IRestClientBuilder { Op = "supportedCountries", NoAuth = true };

            var phaxio = new PhaxioClient(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            var countries = phaxio.ListSupportedCountries();

            var expectedCountries = PocoFixtures.GetTestSupportedCountries();

            Assert.AreEqual(expectedCountries.Count(), countries.Count(), "Number should be the same");
            Assert.AreEqual(expectedCountries["Canada"].PricePerPage, countries["Canada"].PricePerPage, "PricePerPage should be the same");
        }
    }
}
