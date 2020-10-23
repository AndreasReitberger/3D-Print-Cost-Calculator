using System;

namespace PrintCostCalculator3d.Utilities
{
    public static class TimestampHelper
    {
        public static string GetTimestamp()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }
    }
}
