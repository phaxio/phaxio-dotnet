using IniParser;
using IniParser.Model;
using System;

namespace Phaxio.Tests.Helpers
{
    public class KeyManager
    {
        private IniData data;

        public KeyManager ()
        {
            var parser = new FileIniDataParser();
            data = parser.ReadFile(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "keys.ini");
        }

        public string this[string key]
        {
            get
            {
                return data["Phaxio"][key];
            }
        }
    }
}
