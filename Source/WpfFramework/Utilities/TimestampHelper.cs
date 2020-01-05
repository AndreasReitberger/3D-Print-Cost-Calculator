using System;

namespace WpfFramework.Utilities
{
    public static class TimestampHelper
    {
        public static string GetTimestamp()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }
    }
}
