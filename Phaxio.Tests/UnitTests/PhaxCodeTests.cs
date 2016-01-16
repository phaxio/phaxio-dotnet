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
    public class PhaxCodeTests
    {
        [Test]
        public void UnitTests_PhaxCodeCreateWithUrlRequestWorks()
        {
            var clientBuilder = new IRestClientBuilder { Op = "createPhaxCodeUrl" };

            var phaxio = new Phaxio(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            var phaxCodeUrl = phaxio.CreatePhaxCode();

            var expectedPhaxCodeUrl = PocoFixtures.GetTestPhaxCodeUrl();

            Assert.AreEqual(expectedPhaxCodeUrl, phaxCodeUrl.Address, "URLs should be the same.");
        }

        [Test]
        public void UnitTests_PhaxCodeCreateWithUrlAndMetadataRequestWorks()
        {
            var metadata = "key=value";

            Action<IRestRequest> requestAsserts = req =>
            {
                Assert.AreEqual(req.Parameters[2].Value, metadata);
            };

            var clientBuilder = new IRestClientBuilder { Op = "createPhaxCodeUrl", RequestAsserts = requestAsserts };

            var phaxio = new Phaxio(IRestClientBuilder.TEST_KEY, IRestClientBuilder.TEST_SECRET, clientBuilder.Build());

            var phaxCodeUrl = phaxio.CreatePhaxCode(metadata);

            var expectedPhaxCodeUrl = PocoFixtures.GetTestPhaxCodeUrl();

            Assert.AreEqual(expectedPhaxCodeUrl, phaxCodeUrl.Address, "URLs should be the same.");
        }
    }
}
