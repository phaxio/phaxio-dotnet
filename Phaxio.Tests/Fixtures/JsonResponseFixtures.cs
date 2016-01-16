﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phaxio.Tests
{
    public class JsonResponseFixtures
    {
        public static Dictionary<string, string> Fixtures = new Dictionary<string, string>
        {
            {
                "accountStatus",
                @"{
                    ""success"":true,
                    ""message"":""Account status retrieved successfully"",
                    ""data"": {
                        ""faxes_sent_this_month"":120,
                        ""faxes_sent_today"":10,
                        ""balance"":3000
                    }
                }"
            },
            {
                "areaCodes",
                @"{
                    ""success"": true,
                    ""message"": ""295 area codes available."",
                    ""data"": {
                        ""201"": {
                            ""city"": ""Bayonne, Jersey City, Union City"",
                            ""state"": ""New Jersey""
                        },
                        ""202"": {
                            ""city"": ""Washington"",
                            ""state"": ""District Of Columbia""
                        }
                    }
                }"
            },
            {
                "faxCancel",
                @"{
                    ""success"":true,
                    ""message"":""Fax canceled successfully.""
                }"
            }
        };
    }
}