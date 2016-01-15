using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
