using System;
using NUnit.Framework;
using Phaxio.ThinRestClient;
using Phaxio.Tests.Helpers;
using System.Collections.Generic;
using System.Linq;
using Phaxio.Entities;
using Phaxio.Resources.V2;

namespace Phaxio.Tests.UnitTests.V2
{
    using PhoneNumber = Resources.V2.PhoneNumber;

    [TestFixture]
    public class NumberTests
    {
        [Test]
        public void UnitTests_V2_PhoneNumber_Create()
        {
            Action<IRestRequest> parameterAsserts = req =>
            {
                var parameters = ParametersHelper.ToDictionary(req.Parameters);

                Assert.AreEqual("1", parameters["country_code"]);
                Assert.AreEqual("808", parameters["area_code"]);
                Assert.AreEqual("http://example.com", parameters["callback_url"]);
            };

            var requestAsserts = new RequestAsserts()
                .Auth()
                .Post()
                .Custom(parameterAsserts)
                .Resource("phone_numbers")
                .Build();

            var restClient = new RestClientBuilder()
                .WithRequestAsserts(requestAsserts)
                .AsJson()
                .Content(JsonResponseFixtures.FromFile("V2/provision_number"))
                .Ok()
                .Build<Response<PhoneNumber>>();

            var phaxio = new PhaxioClient(RestClientBuilder.TEST_KEY, RestClientBuilder.TEST_SECRET, restClient);

            var number = phaxio.PhoneNumber.Create(
                countryCode: "1",
                areaCode: "808",
                callbackUrl: "http://example.com"
            );

            var lastBilledAt = Convert.ToDateTime("2016-06-16T15:45:32.000-06:00");
            var provisionedAt = Convert.ToDateTime("2016-06-16T15:45:32.000-06:00");

            Assert.AreEqual("+18475551234", number.Number);
            Assert.AreEqual("Northbrook", number.City);
            Assert.AreEqual("Illinois", number.State);
            Assert.AreEqual("United States", number.Country);
            Assert.AreEqual(200, number.Cost);
            Assert.AreEqual(lastBilledAt, number.LastBilled);
            Assert.AreEqual(provisionedAt, number.Provisioned);
            Assert.AreEqual("http://example.com", number.CallbackUrl);
        }

        [Test]
        public void UnitTests_V2_PhoneNumber_Retrieve()
        {
            var requestAsserts = new RequestAsserts()
                .Auth()
                .Get()
                .Resource("phone_numbers/+18475551234")
                .Build();

            var restClient = new RestClientBuilder()
                .WithRequestAsserts(requestAsserts)
                .AsJson()
                .Content(JsonResponseFixtures.FromFile("V2/provision_number"))
                .Ok()
                .Build<Response<PhoneNumber>>();

            var phaxio = new PhaxioClient(RestClientBuilder.TEST_KEY, RestClientBuilder.TEST_SECRET, restClient);

            var number = phaxio.PhoneNumber.Retrieve("+18475551234");

            var lastBilledAt = Convert.ToDateTime("2016-06-16T15:45:32.000-06:00");
            var provisionedAt = Convert.ToDateTime("2016-06-16T15:45:32.000-06:00");

            Assert.AreEqual("+18475551234", number.Number);
            Assert.AreEqual("Northbrook", number.City);
            Assert.AreEqual("Illinois", number.State);
            Assert.AreEqual("United States", number.Country);
            Assert.AreEqual(200, number.Cost);
            Assert.AreEqual(lastBilledAt, number.LastBilled);
            Assert.AreEqual(provisionedAt, number.Provisioned);
            Assert.AreEqual("http://example.com", number.CallbackUrl);
        }

        [Test]
        public void UnitTests_V2_PhoneNumber_List()
        {
            Action<IRestRequest> parameterAsserts = req =>
            {
                var parameters = ParametersHelper.ToDictionary(req.Parameters);

                Assert.AreEqual("1", parameters["country_code"]);
                Assert.AreEqual("808", parameters["area_code"]);
                Assert.AreEqual(25, parameters["per_page"]);
                Assert.AreEqual(1, parameters["page"]);
            };

            var requestAsserts = new RequestAsserts()
                .Auth()
                .Get()
                .Custom(parameterAsserts)
                .Resource("phone_numbers")
                .Build();

            var restClient = new RestClientBuilder()
                .WithRequestAsserts(requestAsserts)
                .AsJson()
                .Content(JsonResponseFixtures.FromFile("V2/list_numbers"))
                .Ok()
                .Build<Response<List<PhoneNumber>>>();

            var phaxio = new PhaxioClient(RestClientBuilder.TEST_KEY, RestClientBuilder.TEST_SECRET, restClient);

            var list = phaxio.PhoneNumber.List(
                countryCode: "1",
                areaCode: "808",
                page: 1,
                perPage: 25
            );

            Assert.AreEqual(2, list.Count());

            var number = list.First();
            Assert.AreEqual("+18475551234", number.Number);
        }

        [Test]
        public void UnitTests_V2_Numbers_Release()
        {
            var requestAsserts = new RequestAsserts()
                .Auth()
                .Delete()
                .Resource("phone_numbers/+18475551234")
                .Build();

            var restClient = new RestClientBuilder()
                .WithRequestAsserts(requestAsserts)
                .AsJson()
                .Content(JsonResponseFixtures.FromFile("V2/generic_success"))
                .Ok()
                .Build<Response<Object>>();

            var phaxio = new PhaxioClient(RestClientBuilder.TEST_KEY, RestClientBuilder.TEST_SECRET, restClient);

            var number = new PhoneNumber { Number = "+18475551234", PhaxioClient = phaxio };

            number.Release();
        }

        [Test]
        public void UnitTests_V2_Numbers_ListAreaCodes()
        {
            Action<IRestRequest> parameterAsserts = req =>
            {
                var parameters = ParametersHelper.ToDictionary(req.Parameters);

                Assert.AreEqual(true, parameters["toll_free"]);
                Assert.AreEqual("1", parameters["country_code"]);
                Assert.AreEqual("US", parameters["country"]);
                Assert.AreEqual("WA", parameters["state"]);
                Assert.AreEqual(25, parameters["per_page"]);
                Assert.AreEqual(1, parameters["page"]);
            };

            var requestAsserts = new RequestAsserts()
                .Get()
                .Custom(parameterAsserts)
                .Resource("public/area_codes")
                .Build();

            var restClient = new RestClientBuilder()
                .WithRequestAsserts(requestAsserts)
                .AsJson()
                .Content(JsonResponseFixtures.FromFile("V2/list_area_codes"))
                .Ok()
                .Build<Response<List<AreaCode>>>();

            var phaxio = new PhaxioClient(RestClientBuilder.TEST_KEY, RestClientBuilder.TEST_SECRET, restClient);

            var list = phaxio.Public.AreaCode.List(
                countryCode: "1",
                tollFree: true,
                country: "US",
                state: "WA",
                page: 1,
                perPage: 25
            );

            Assert.AreEqual(3, list.Count());

            var areaCode = list.First();
            Assert.AreEqual("1", areaCode.CountryCode);
            Assert.AreEqual("201", areaCode.AreaCodeNumber);
            Assert.AreEqual("Bayonne, Jersey City, Union City", areaCode.City);
            Assert.AreEqual("New Jersey", areaCode.State);
            Assert.AreEqual("United States", areaCode.Country);
            Assert.IsFalse(areaCode.TollFree);
        }
    }
}