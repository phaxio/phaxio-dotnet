using IniParser;
using IniParser.Model;
using Phaxio.Entities;
using System;
using System.IO;

namespace Phaxio.Examples.Basic
{
    class Example
    {
        static void Main(string[] args)
        {
            var parser = new FileIniDataParser();

            IniData config = parser.ReadFile(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "keys.ini");

            var phaxio = new Phaxio(config["Phaxio"]["api_key"], config["Phaxio"]["api_secret"]);

            //var account = phaxio.GetAccountStatus();

            //Console.WriteLine(account.Balance);

            var pdf = new FileInfo("C:\\temp\\test.pdf");

            var request = new FaxRequest { ToNumber = "+18088675309", File = pdf };

            var faxId = phaxio.SendFax(request);

            Console.WriteLine(faxId);

            Console.ReadKey();
        }
    }
}
