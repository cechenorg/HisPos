using System;
using System.Globalization;

namespace His_Pos.Service
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// To the simple taiwan date.
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <returns></returns>

        /*
         * 將Datetime轉為民國日期string
         */

        public static string ToSimpleTaiwanDate(DateTime datetime)
        {
            if (datetime == DateTime.MinValue)
                return datetime.Year + "/"+ datetime.Month +"/" + datetime.Day;
            var taiwanCalendar = new TaiwanCalendar();
            var year = taiwanCalendar.GetYear(datetime).ToString().PadLeft(3, '0');
            var month = CheckDateLessTen(datetime.Month.ToString());
            var day = CheckDateLessTen(datetime.Day.ToString());
            return $"{year}/{month}/{day}";
        }

        /*
        * 將Datetime string轉為西元Datetime
        */

        public static DateTime ToUsDate(string datetime)
        {
            string year;
            string month;
            string date;
            if (string.IsNullOrEmpty(datetime))
                return new DateTime();
            if (datetime.Contains("/"))
            {
                year = datetime.Split('/')[0];
                month = datetime.Split('/')[1];
                date = datetime.Split('/')[2];
            }
            else
            {
                year = datetime.Split('-')[0];
                month = datetime.Split('-')[1];
                date = datetime.Split('-')[2];
            }
           
            if (year.StartsWith("0") || year.Length == 3)
                year = (int.Parse(datetime.Substring(0, 4)) + 1911).ToString();
            else
                year = int.Parse(datetime.Substring(0, 4)).ToString();

            if (int.Parse(year) < 1911)
                year += 1911;
            datetime = year + "/" + month + "/" + date;
            return DateTime.ParseExact(datetime, "yyyy/MM/dd", CultureInfo.InvariantCulture);
        }
 
        /*
         * 計算年齡
         */

        public static double CalculateAge(DateTime birthDate)
        {
            var month = birthDate.Month.ToString();
            var day = birthDate.Day.ToString();
            month = CheckDateLessTen(month);
            day = CheckDateLessTen(day);
            birthDate = DateTime.ParseExact(birthDate.Year + "/" + month + "/" + day, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            var ts = DateTime.Now - birthDate;
            var age = ts.TotalDays / 365.2422;
            return age;
        }

        private static string CheckDateLessTen(string dateStr)
        {
            return dateStr.PadLeft(2, '0');
        }

        /*
        Data:yy/mm/dd => yyy/mm/dd 
        */
        public static string YearFormatTransfer(string date) {
            if (date.Split('/')[0].Length == 2)
            {
                return "0" + date;
            }
            else
                return  date;
        }

        public static string ConvertToTaiwanCalender(DateTime d,bool needSplit)
        {
            var year = (d.Year - 1911).ToString().PadLeft(3, '0');
            var month = (d.Month).ToString().PadLeft(2, '0');
            var day = (d.Day).ToString().PadLeft(2, '0');
            if (needSplit)
                return year + "/" + month + "/" + day;
            return year + month + day;
        } 
    }
}