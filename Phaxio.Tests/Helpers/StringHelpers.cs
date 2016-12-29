using System;
using System.Linq;

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
