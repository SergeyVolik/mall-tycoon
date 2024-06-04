using System;
using System.Linq;
using System.Text;

namespace Prototype
{
    public static class TextUtils
    {
        public static string IntToText(float numberOfIntems)
        {
            var thousand = numberOfIntems / 1000f;

            if (thousand >= 1 && thousand < 1000)
            {
                return $"{thousand.ToString("0.0")}K";
            }

            var milions = thousand / 1000f;

            if (milions >= 1 && milions < 1000)
            {
                return $"{milions.ToString("0.0")}M";
            }

            var bilions = milions / 1000f;

            if (bilions >= 1)
            {
                return $"{bilions.ToString("0.0")}B";
            }

            return numberOfIntems.ToString();
        }

        private static StringBuilder strBuilder = new StringBuilder();

        public static string SplitBy3Number(float value)
        {
            strBuilder.Clear();
            var result = SplitNumber((int)value);

            foreach ( var item in result ) {
                strBuilder.Append(item);
                strBuilder.Append(" ");
            }
            return strBuilder.ToString();
        }

        static int[] SplitNumber(int value)
        {
            int length = (int)(1 + Math.Log(value, 1000));
            var result = from n in Enumerable.Range(1, length)
                         select ((int)(value / Math.Pow(1000, length - n))) % 1000;
            return result.ToArray();
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
