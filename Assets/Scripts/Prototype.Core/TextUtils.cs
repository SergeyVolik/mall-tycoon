using System;

namespace Prototype
{
    public static class TextUtils
    {
        public static string IntToText(int numberOfIntems)
        {
            var Thousand = numberOfIntems / 1000f;

            if (Thousand >= 1)
            {
                return $"{Thousand.ToString("0.0")}K";
            }

            return numberOfIntems.ToString();
        }

        public static string TimeFormat(TimeSpan time)
        {
            if (time.Hours != 0)
            {
                return $"{time.Hours}h {time.Minutes}m ";
            }
            else if (time.Minutes != 0)
            {
                return $"{time.Minutes}m {time.Seconds}s ";
            }
            else if (time.Seconds != 0)
            {
                return $"{time.Seconds}s ";
            }

            return "0s";
        }
    }
}
