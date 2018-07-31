using System;
using System.IO;
using Phaxio.Tests.Helpers;

namespace Phaxio.Tests
{
    class BinaryFixtures
    {
        public static byte[] GetTestPhaxCode ()
        {
            var hex = "89504E470D0A1A0A0000000D49484452000001180000003E08020000007F6E9344000000E84944415478DAEDD8410A8330104051477AFF2B4F1742118684D1B8A8F0DEAA040D31FAB11899B9016B765B0042022181900021C1933E7528227EBF8F6F7AC7C8F9FB5E1DA933CCCFADEE1D331AA93374CEAD577DF5DCCE0CA3BDEA5FC57CB5F78EECEFCCCA1DEFAFEAEA9A3B4FECE82AE6BBD7B99BDE48E0AF1D0809840408098404420221014202218190404880904048202410122024101208098404080984044202210142022181900021819040482024404820241012080910120809840442028404420221819000218190404820244048202478B1C84CBB00DE482024101220241012FC932F32FE7577A9DABDFA0000000049454E44AE426082";

            return toBytesFromHex(hex);
        }

        public static FileInfo getTestPdfFile ()
        {
            return new FileInfo(getQualifiedPath("test.pdf"));
        }

        public static byte[] GetTestPdf()
        {
            return File.ReadAllBytes(getQualifiedPath("test.pdf"));
        }

        private static string getQualifiedPath(string filename)
        {
            return App.BaseDirectory() + "Fixtures" + Path.DirectorySeparatorChar + filename;
        }

        private static byte[] toBytesFromHex (string hex)
        {
            byte[] bytes = new byte[hex.Length / 2];

            for (int i = 0; i < hex.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return bytes;
        }
    }
}
