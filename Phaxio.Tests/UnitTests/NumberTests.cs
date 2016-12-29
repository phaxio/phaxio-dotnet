using System;
using NUnit.Framework;
using Phaxio.Tests.Fixtures;
using Phaxio.ThinRestClient;
using Phaxio.Tests.Helpers;
using Phaxio.Entities.Internal;
using System.Collections.Generic;
using Phaxio.Entities;
using System.Linq;

namespace Phaxio.Tests
{
    [TestFixture]
    public class NumberTests
    {
        [Test]
        public void UnitTests_Numbers_Provision()
        {
            var areaCode = "808";

            Action<IRestRequest> requestAsserts = req =>
            {
                Assert.AreEqual(req.Parameters[2].Value, areaCode);
            };

            var clientBuilder = new IRestClientBuilder { Op = "provisionNumber", RequestAsserts = requestAsserts };
            var phaxio = new PhaxioClient(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            var actualNumber = phaxio.ProvisionNumber(areaCode);

            var expectedNumber = PocoFixtures.GetTestPhoneNumber();

            Assert.AreEqual(expectedNumber.Number, actualNumber.Number, "Number should be the same");
        }

        [Test]
        public void UnitTests_V2_Numbers_Provision()
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
                .Build<Response<PhoneNumberV2>>();

            var phaxio = new Phaxio(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, restClient);

            var number = phaxio.ProvisionNumber(
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
        public void UnitTests_V2_Numbers_Get()
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
                .Build<Response<PhoneNumberV2>>();

            var phaxio = new Phaxio(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, restClient);

            var number = phaxio.GetNumberInfo("+18475551234");

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
        public void UnitTests_V2_Numbers_List()
        {
            Action<IRestRequest> parameterAsserts = req =>
            {
                var parameters = ParametersHelper.ToDictionary(req.Parameters);

                Assert.AreEqual("1", parameters["country_code"]);
                Assert.AreEqual("808", parameters["area_code"]);
                Assert.AreEqual(25, parameters["per_page"]);
                Assert.AreEqual(400, parameters["page"]);
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
                .Build<Response<List<PhoneNumberV2>>>();

            var phaxio = new Phaxio(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, restClient);

            var list = phaxio.ListAccountNumbers(
                countryCode: "1",
                areaCode: "808",
                page: 400,
                perPage: 25
            );

            Assert.AreEqual(2, list.Data.Count);
            Assert.AreEqual(1, list.PagingInfo.Page);
            Assert.AreEqual(25, list.PagingInfo.PerPage);
            Assert.AreEqual(2, list.PagingInfo.Total);

            var number = list.Data.First();
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

            var phaxio = new Phaxio(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, restClient);

            var result = phaxio.ReleaseNumber("+18475551234");

            Assert.IsTrue(result.Success);
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
                Assert.AreEqual(400, parameters["page"]);
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

            var phaxio = new Phaxio(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, restClient);

            var list = phaxio.ListAreaCodes(
                countryCode: "1",
                tollFree: true,
                country: "US",
                state: "WA",
                page: 400,
                perPage: 25
            );

            Assert.AreEqual(3, list.Data.Count);
            Assert.AreEqual(1, list.PagingInfo.Page);
            Assert.AreEqual(3, list.PagingInfo.PerPage);
            Assert.AreEqual(270, list.PagingInfo.Total);

            var areaCode = list.Data.First();
            Assert.AreEqual("1", areaCode.CountryCode);
            Assert.AreEqual("201", areaCode.AreaCodeNumber);
            Assert.AreEqual("Bayonne, Jersey City, Union City", areaCode.City);
            Assert.AreEqual("New Jersey", areaCode.State);
            Assert.AreEqual("United States", areaCode.Country);
            Assert.IsFalse(areaCode.TollFree);
        }

        [Test]
        public void UnitTests_Numbers_ProvisionWithCallBack()
        {
            var areaCode = "808";
            var callback = "http://example.com/callback";

            Action<IRestRequest> requestAsserts = req =>
            {
                Assert.AreEqual(req.Parameters[2].Value, areaCode);
                Assert.AreEqual(req.Parameters[3].Value, callback);
            };

            var clientBuilder = new IRestClientBuilder { Op = "provisionNumber", RequestAsserts = requestAsserts };
            var phaxio = new PhaxioClient(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            var actualNumber = phaxio.ProvisionNumber(areaCode, callback);

            var expectedNumber = PocoFixtures.GetTestPhoneNumber();

            Assert.AreEqual(expectedNumber.Number, actualNumber.Number, "Number should be the same");
        }

        [Test]
        public void UnitTests_Numbers_ListNumbers()
        {
            var clientBuilder = new IRestClientBuilder { Op = "numberList" };
            var phaxio = new PhaxioClient(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            var actualNumbers = phaxio.ListNumbers();

            var expectedNumbers = PocoFixtures.GetTestPhoneNumbers();

            Assert.AreEqual(expectedNumbers.Count, actualNumbers.Count, "Number should be the same");
        }

        [Test]
        public void UnitTests_Numbers_ListNumbersWithOptions()
        {
            var areaCode = "808";
            var number = "8088675309";

            Action<IRestRequest> requestAsserts = req =>
            {
                Assert.AreEqual(req.Parameters[2].Value, areaCode);
                Assert.AreEqual(req.Parameters[3].Value, number);
            };

            var clientBuilder = new IRestClientBuilder { Op = "numberList", RequestAsserts = requestAsserts };
            var phaxio = new PhaxioClient(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            var actualNumbers = phaxio.ListNumbers(areaCode, number);

            var expectedNumbers = PocoFixtures.GetTestPhoneNumbers();

            Assert.AreEqual(expectedNumbers.Count, actualNumbers.Count, "Number should be the same");
        }

        [Test]
        public void UnitTests_Numbers_Release()
        {
            var number = "8088675309";

            Action<IRestRequest> requestAsserts = req =>
            {
                Assert.AreEqual(req.Parameters[2].Value, number);
            };

            var clientBuilder = new IRestClientBuilder { Op = "releaseNumber", RequestAsserts = requestAsserts };
            var phaxio = new PhaxioClient(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            var result = phaxio.ReleaseNumber(number);

            Assert.True(result.Success, "Should be success.");
        }
    }
}