using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Extention
{
    public static class DateTimeFormatExtention
    {
        //西元轉民國年
        public static string ToTaiwanDateTime(this DateTime date)
        {
            CultureInfo culture = new CultureInfo("zh-TW");
            culture.DateTimeFormat.Calendar = new TaiwanCalendar();
            string zhDate = date.ToString("yyy/MM/dd", culture);

            return zhDate;
        }
    }
}
