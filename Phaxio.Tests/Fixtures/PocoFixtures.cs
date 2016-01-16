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
    }
}
