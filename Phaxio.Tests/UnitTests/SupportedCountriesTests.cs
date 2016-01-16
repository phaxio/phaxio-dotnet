using NUnit.Framework;
using Phaxio.Tests.Fixtures;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phaxio.Tests.UnitTests
{
    [TestFixture]
    public class SupportedCountriesTests
    {
        [Test]
        public void UnitTests_SupportedCountriesRequestWorks()
        {
            var clientBuilder = new IRestClientBuilder { Op = "supportedCountries", NoAuth = true };

            var phaxio = new Phaxio(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            var countries = phaxio.GetSupportedCountries();

            var expectedCountries = PocoFixtures.GetTestSupporteCountries();

            Assert.AreEqual(expectedCountries.Count(), countries.Count(), "Number should be the same");
            Assert.AreEqual(expectedCountries["Canada"].PricePerPage, countries["Canada"].PricePerPage, "PricePerPage should be the same");
        }
    }
}
