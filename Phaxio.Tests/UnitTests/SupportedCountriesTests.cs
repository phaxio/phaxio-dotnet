using NUnit.Framework;
using Phaxio.Entities;
using Phaxio.Entities.Internal;
using Phaxio.Tests.Fixtures;
using Phaxio.Tests.Helpers;
using Phaxio.ThinRestClient;
using System;
using System.Collections.Generic;
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

        [Test]
        public void UnitTests_V2_SupportedCountries()
        {
            Action<IRestRequest> parameterAsserts = req =>
            {
                var parameters = ParametersHelper.ToDictionary(req.Parameters);

                Assert.AreEqual(25, parameters["per_page"]);
                Assert.AreEqual(400, parameters["page"]);
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

            var phaxio = new Phaxio(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, restClient);

            var list = phaxio.ListSupportedCountries(
                page: 400,
                perPage: 25
            );

            Assert.AreEqual(3, list.Data.Count);
            Assert.AreEqual(1, list.PagingInfo.Page);
            Assert.AreEqual(3, list.PagingInfo.PerPage);
            Assert.AreEqual(47, list.PagingInfo.Total);

            var country = list.Data.First();
            Assert.AreEqual("United States", country.Name);
            Assert.AreEqual("US", country.Alpha2);
            Assert.AreEqual("1", country.CountryCode);
            Assert.AreEqual(7, country.PricePerPage);
            Assert.AreEqual("full", country.SendSupport);
            Assert.AreEqual("fullest", country.ReceiveSupport);
        }
    }
}