using System;
using System.Collections.Generic;
using System.Text;

namespace WotlkClient.Shared
{
    public static class Time
    {
        public static int UnixTime()
        {
            TimeSpan t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
            int timestamp = (int)t.TotalSeconds;
            return timestamp;
        }

        public static string GetTime()
        {
            string TimeInString = "";

            // Get current time
            int hour = DateTime.Now.Hour;
            int min = DateTime.Now.Minute;
            int sec = DateTime.Now.Second;

            // Format current time into string
            TimeInString = (hour < 10) ? "0" + hour.ToString() : hour.ToString();
            TimeInString += ":" + ((min < 10) ? "0" + min.ToString() : min.ToString());
            TimeInString += ":" + ((sec < 10) ? "0" + sec.ToString() : sec.ToString());

            return TimeInString;
        }

        public static string GetDate()
        {
            string DateInString = "";
            return "";

        }
    }
}

