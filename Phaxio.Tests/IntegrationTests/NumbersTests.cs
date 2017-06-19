using NUnit.Framework;
using Phaxio.Tests.Helpers;
using System.Linq;
using System.Threading;

namespace Phaxio.Tests.IntegrationTests.V2
{
    [TestFixture, Explicit]
    public class NumbersTests
    {
        [Test]
        public void IntegrationTests_V2_Public_AreaCode_List()
        {
            var config = new KeyManager();

            var phaxio = new PhaxioClient(config["api_key"], config["api_secret"]);

            var areaCodes = phaxio.Public.AreaCode.List(state: "HI", country: "US");

            Assert.Greater(areaCodes.Count(), 0, "There should be some area codes");
        }

        [Test]
        public void IntegrationTests_V2_Numbers_BasicScenario()
        {
            var config = new KeyManager();

            var phaxio = new PhaxioClient(config["api_key"], config["api_secret"]);

            // Find area codes to provision a number in
            var areaCodes = phaxio.Public.AreaCode.List(country: "US", state: "DE");

            Assert.Greater(areaCodes.Count(), 0, "There should be some area codes");

            var areaCode = areaCodes.First();

            Thread.Sleep(1000);

            // Provision a number
            var provisionedNumber = phaxio.PhoneNumber.Create(areaCode.AreaCodeNumber, areaCode.CountryCode);

            Thread.Sleep(1000);

            // Check to see if the number's listed on the account
            var accountNumbers = phaxio.PhoneNumber.List();

            if (!accountNumbers.Any(n => n.Number == provisionedNumber.Number))
            {
                throw new AssertionException("ListNumbers should return newly provisioned number.");
            }

            Thread.Sleep(1000);

            // Release the number
            provisionedNumber.Release();

            Thread.Sleep(1000);

            // Check to see if the number's still listed on the account
            accountNumbers = phaxio.PhoneNumber.List();

            if (accountNumbers.Any(n => n.Number == provisionedNumber.Number))
            {
                throw new AssertionException("ListNumbers should not return released number.");
            }
        }
    }
}
