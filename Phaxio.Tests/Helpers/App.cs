using System;

namespace Phaxio.Tests.Helpers
{
    public class App
    {

#if CORE
        public static string BaseDirectory()
        {
            return AppContext.BaseDirectory;
        }
#else
        public static string BaseDirectory()
        {
            return AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        }
#endif
    }
}
