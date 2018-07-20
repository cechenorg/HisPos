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
            if (datetime.Length >= 10)
                return Convert.ToDateTime(datetime.Substring(0, 10));
            if (datetime.Substring(0, 3).Contains("/"))
                datetime = "0" + datetime;
            var dt = DateTime.ParseExact(datetime, "yyy/MM/dd", CultureInfo.InvariantCulture).AddYears(1911);
            return dt;
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

        public static string BirthdayFormatConverter(string birthday)
        {
            var year = birthday.Substring(0, 3).StartsWith("0") ? birthday.Substring(1, 2) : birthday.Substring(0, 3);
            return year + "/" + birthday.Substring(3, 2) + "/" + birthday.Substring(5, 2);
        }

        public static string UsToTaiwan(string ustring)
        {
            string[] split = ustring.Split('/');
            if (split[1].Length == 1) split[1] = "0" + split[1];
            if (split[2].Length == 1) split[2] = "0" + split[2];
            return (Convert.ToInt32(split[0]) - 1911).ToString() + split[1] + split[2];
        }

        private static string CheckDateLessTen(string dateStr)
        {
            return dateStr.PadLeft(2, '0');
        }
    }
}