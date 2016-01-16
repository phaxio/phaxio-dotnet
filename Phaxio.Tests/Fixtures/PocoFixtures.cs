using Phaxio.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phaxio.Tests.Fixtures
{
    class PocoFixtures
    {
        public static Account GetTestAccount()
        {
            return new Account
            {
                FaxesSentThisMonth = 120,
                FaxesSentToday = 10,
                Balance = 3000
            };
        }

        public static Dictionary<string, CityState> GetTestAreaCodes()
        {
            return new Dictionary<string, CityState>
            {
                {"201", new CityState { City = "Bayonne, Jersey City, Union City", State = "New Jersey" }},
                {"202", new CityState { City = "Washington", State = "District Of Columbia"  }}
            };
        }

        public static Dictionary<string, Pricing> GetTestSupportedCountries()
        {
            return new Dictionary<string, Pricing>
            {
                {"United States", new Pricing { PricePerPage = 7 }},
                {"Canada", new Pricing { PricePerPage = 7}},
                {"Argentina", new Pricing { PricePerPage = 10 }}
            };
        }

        public static PhoneNumber GetTestPhoneNumber ()
        {
            return new PhoneNumber
            {
                Number = "8475551234",
                City = "Northbrook",
                State = "Illinois",
                Cost = 200,
                LastBilled = Convert.ToDateTime("2013-11-12 11:39:05"),
                Provisioned = Convert.ToDateTime("2013-11-12 11:39:05")
            };
        }
    }
}
