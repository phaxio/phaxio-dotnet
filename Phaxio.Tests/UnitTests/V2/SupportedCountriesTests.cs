using NUnit.Framework;
using Phaxio.Entities;
using Phaxio.Tests.Helpers;
using Phaxio.ThinRestClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Phaxio.Tests.UnitTests.UnitTests.V2
{
    [TestFixture]
    public class SupportedCountriesTests
    {
        [Test]
        public void UnitTests_V2_SupportedCountries()
        {
            Action<IRestRequest> parameterAsserts = req =>
            {
                var parameters = ParametersHelper.ToDictionary(req.Parameters);

                Assert.AreEqual(25, parameters["per_page"]);
                Assert.AreEqual(1, parameters["page"]);
            };

            var requestAsserts = new RequestAsserts()
                .Get()
                .Custom(parameterAsserts)
                .Resource("public/countries")
                .Build();

            var restClient = new RestClientBuilder()
                .WithRequestAsserts(requestAsserts)
                .AsJson()
                .Content(JsonResponseFixtures.FromFile("V2/supported_countries"))
                .Ok()
                .Build<Response<List<Country>>>();

            var phaxio = new PhaxioContext(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, restClient);

            var list = phaxio.Public.SupportedCountry.List(
                page: 1,
                perPage: 25
            );

            var country = list.First();
            Assert.AreEqual("United States", country.Name);
            Assert.AreEqual("US", country.Alpha2);
            Assert.AreEqual("1", country.CountryCode);
            Assert.AreEqual(7, country.PricePerPage);
            Assert.AreEqual("full", country.SendSupport);
            Assert.AreEqual("fullest", country.ReceiveSupport);
        }
    }
}