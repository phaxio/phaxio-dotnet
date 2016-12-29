using System;
using NUnit.Framework;
using Phaxio.ThinRestClient;
using Phaxio.Entities;
using Phaxio.Tests.Helpers;
using Phaxio.Entities.Internal;
using System.Collections.Generic;

namespace Phaxio.Tests
{
    [TestFixture]
    public class FaxListTests
    {
        [Test]
        public void UnitTests_V2_Fax_List()
        {
            var createdBeforeTxt = "2016-02-01T00:00:00Z";
            var createdBefore = DateTime.Parse(createdBeforeTxt).ToUniversalTime();

            var createdAfterTxt = "2016-01-01T00:00:00Z";
            var createdAfter = DateTime.Parse(createdAfterTxt).ToUniversalTime();

            Action<IRestRequest> parameterAsserts = req =>
            {
                var parameters = ParametersHelper.ToDictionary(req.Parameters);

                Assert.AreEqual(createdBeforeTxt, parameters["created_before"]);
                Assert.AreEqual(createdAfterTxt, parameters["created_after"]);
                Assert.AreEqual("sent", parameters["direction"]);
                Assert.AreEqual("finished", parameters["status"]);
                Assert.AreEqual("1234", parameters["phone_number"]);
                Assert.AreEqual("value", parameters["tag[key]"]);
                Assert.AreEqual(25, parameters["per_page"]);
                Assert.AreEqual(400, parameters["page"]);
            };
       
            var requestAsserts = new RequestAsserts()
                .Auth()
                .Get()
                .Custom(parameterAsserts)
                .Resource("faxes")
                .Build();

            var restClient = new RestClientBuilder()
                .WithRequestAsserts(requestAsserts)
                .AsJson()
                .Content(JsonResponseFixtures.FromFile("V2/fax_list"))
                .Ok()
                .Build<Response<List<FaxInfo>>>();

            var phaxio = new Phaxio(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, restClient);

            var list = phaxio.ListFaxes(
                createdBefore: createdBefore,
                createdAfter: createdAfter,
                direction: "sent",
                status: "finished",
                phoneNumber: "1234",
                tagName: "key",
                tagValue: "value",
                page: 400,
                perPage: 25
            );

            Assert.AreEqual(3, list.Data.Count);
            Assert.AreEqual(1, list.PagingInfo.Page);
            Assert.AreEqual(25, list.PagingInfo.PerPage);
            Assert.AreEqual(3, list.PagingInfo.Total);
        }
    }
}