using System;
using System.Net.Http.Headers;

namespace Phaxio.ThinRestClient
{
    public class BasicAuthorization
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public AuthenticationHeaderValue ToHeader ()
        {
            var authString = string.Format("{0}:{1}", Username, Password);
            var authBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(authString);
            return new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));
        }
    }
}
