using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phaxio.Tests.Helpers
{
    class StringHelpers
    {
        public static string Random(int length)
        {
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var charArray = Enumerable.Repeat(alphabet, length)
                .Select(s => s[random.Next(s.Length)])
                .ToArray();
            return new string(charArray);
        }
    }
}
