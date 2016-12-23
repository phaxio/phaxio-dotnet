using NUnit.Framework;
using Phaxio.Tests.Helpers;
using Phaxio.V2;
using System.Linq;
using System.Threading;

namespace Phaxio.Tests.IntegrationTests.V2
{
    [TestFixture, Explicit]
    public class NumbersTests
    {
        [Test]
        public void IntegrationTests_V2_Numbers_GetAreaCodes()
        {
            var config = new KeyManager();

            var phaxio = new PhaxioV2Client(config["api_key"], config["api_secret"]);

            var areaCodes = phaxio.ListAreaCodes(state: "HI", country: "US");

            Assert.Greater(areaCodes.Data.Count(), 0, "There should be some area codes");
        }

        [Test]
        public void IntegrationTests_V2_Numbers_BasicScenario()
        {
            var config = new KeyManager();

            var phaxio = new PhaxioV2Client(config["api_key"], config["api_secret"]);

            // Find area codes to provision a number in
            var areaCodes = phaxio.ListAreaCodes(country: "US", state: "DE");

            Assert.Greater(areaCodes.Data.Count(), 0, "There should be some area codes");

            var areaCode = areaCodes.Data.First();

            Thread.Sleep(1000);

            // Provision a number
            var provisionedNumber = phaxio.ProvisionNumber(areaCode.AreaCodeNumber, areaCode.CountryCode);

            Thread.Sleep(1000);

            // Check to see if the number's listed on the account
            var accountNumbers = phaxio.ListAccountNumbers();

            if (!accountNumbers.Data.Any(n => n.Number == provisionedNumber.Number))
            {
                throw new AssertionException("ListNumbers should return newly provisioned number.");
            }

            Thread.Sleep(1000);

            // Release the number
            phaxio.ReleaseNumber(provisionedNumber.Number);

            Thread.Sleep(1000);

            // Check to see if the number's still listed on the account
            accountNumbers = phaxio.ListAccountNumbers();

            if (accountNumbers.Data.Any(n => n.Number == provisionedNumber.Number))
            {
                throw new AssertionException("ListNumbers should not return released number.");
            }
        }
    }
}
