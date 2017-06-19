using System;
using NUnit.Framework;
using Phaxio.ThinRestClient;
using Phaxio.Entities;
using Phaxio.Tests.Helpers;
using Phaxio.Entities.Internal;
using System.Collections.Generic;
using System.Linq;

namespace Phaxio.Tests.UnitTests.V2
{
    using Fax = Phaxio.Resources.V2.Fax;
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
                Assert.AreEqual(1, parameters["page"]);
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
                .Build<Response<List<Fax>>>();

            var phaxio = new PhaxioClient(RestClientBuilder.TEST_KEY, RestClientBuilder.TEST_SECRET, restClient);

            var list = phaxio.Fax.List(
                createdBefore: createdBefore,
                createdAfter: createdAfter,
                direction: "sent",
                status: "finished",
                phoneNumber: "1234",
                tagName: "key",
                tagValue: "value",
                page: 1,
                perPage: 25
            );

            Assert.AreEqual(3, list.Count());
        }
    }
}