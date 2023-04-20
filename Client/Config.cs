using System;
using WotlkClient.Constants;

namespace WotlkClient
{
    public static class Config
    {
        public static string Login;
        public static string Password;
        public static string Host;
        public static WoWVersion Version;
        public static long LogFilter;
        public static bool Retail;
        public static bool LogToFile;

        static Config()
        {
            Version.major = 3;
            Version.minor = 3;
            Version.update = 5;
            Version.build = 12340;


            Retail = false;

            LogFilter = 0x0000000000000000;
            LogToFile = true;
        }
    }
}
