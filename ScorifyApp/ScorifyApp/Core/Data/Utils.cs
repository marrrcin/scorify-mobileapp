using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScorifyApp.Core.Data
{
    public static class Utils
    {
        public static string ToApiDateTimeFormat(this DateTime date)
        {
            return string.Format("{0}-{1}-{2}T{3}:{4}:{5}.000+00:00", date.Year,date.Month.ToString("D2"),date.Day.ToString("D2"),date.Hour.ToString("D2"),date.Minute.ToString("D2"),date.Second.ToString("D2"));
        }

        public static DateTime ToDateTime(this long timestamp)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(timestamp).ToLocalTime();
            return dateTime;
        }
    }
}
