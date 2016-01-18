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
    public class AreaCodesTests
    {
        [Test]
        public void UnitTests_AreaCodes_NoOptions()
        {
            var clientBuilder = new IRestClientBuilder { Op = "areaCodes", NoAuth = true };

            var phaxio = new Phaxio(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            var codes = phaxio.GetAreaCodes(tollFree: true);

            var expectedCodes = PocoFixtures.GetTestAreaCodes();

            Assert.AreEqual(expectedCodes.Count(), codes.Count(), "Number should be the same");
            Assert.AreEqual(expectedCodes["201"].City, codes["201"].City, "City should be the same");
            Assert.AreEqual(expectedCodes["201"].State, codes["201"].State, "State should be the same");
        }

        [Test]
        public void UnitTests_AreaCodes_WithTollFree()
        {
            Action<IRestRequest> requestAsserts = req =>
            {
                Assert.AreEqual(req.Parameters[0].Value, true);
            };

            var clientBuilder = new IRestClientBuilder { Op = "areaCodes", NoAuth = true, RequestAsserts = requestAsserts };

            var phaxio = new Phaxio(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            var codes = phaxio.GetAreaCodes(tollFree: true);
        }

        [Test]
        public void UnitTests_AreaCodes_WithState()
        {
            Action<IRestRequest> requestAsserts = req =>
            {
                Assert.AreEqual(req.Parameters[0].Value, "ID");
            };

            var clientBuilder = new IRestClientBuilder { Op = "areaCodes", NoAuth = true, RequestAsserts = requestAsserts };

            var phaxio = new Phaxio(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            var codes = phaxio.GetAreaCodes(state: "ID");
        }
    }
}
