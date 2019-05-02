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

        public static DateTime ConvertDeclareFileDate(string dateStr)
        {
            var year = int.Parse(dateStr.Substring(0,3)) + 1911;
            var month = int.Parse(dateStr.Substring(3, 2));
            var day = int.Parse(dateStr.Substring(5, 2));
            return new DateTime(year,month,day);
        }

        public static string ConvertToTaiwanCalenderWithTime(DateTime d)
        {
            var year = (d.Year - 1911).ToString().PadLeft(3, '0');
            var month = (d.Month).ToString().PadLeft(2, '0');
            var day = (d.Day).ToString().PadLeft(2, '0');
            var hour = (d.Hour).ToString().PadLeft(2, '0');
            var minute = (d.Minute).ToString().PadLeft(2, '0');
            return year + month + day + hour + minute;
        }

        public struct Age
        {
            public int Years;
            public int Months;
            public int Days;
        }
        public static Age CalculateAgeToMonth(DateTime birthDate)
        {
            int years = DateTime.Today.Year - birthDate.Year;
            int months = 0;
            int days = 0;

            // Check if the last year, was a full year.
            if (DateTime.Today < birthDate.AddYears(years) && years != 0)
            {
                years--;
            }

            // Calculate the number of months.
            birthDate = birthDate.AddYears(years);

            if (birthDate.Year == DateTime.Today.Year)
            {
                months = DateTime.Today.Month - birthDate.Month;
            }
            else
            {
                months = (12 - birthDate.Month) + DateTime.Today.Month;
            }

            // Check if last month was a complete month.
            if (DateTime.Today < birthDate.AddMonths(months) && months != 0)
            {
                months--;
            }

            // Calculate the number of days.
            birthDate = birthDate.AddMonths(months);

            days = (DateTime.Today - birthDate).Days;
            Age result;
            result.Years = years;
            result.Months = months;
            result.Days = days;
            return result;
        }
        public static string NullableDateToTWCalender(DateTime? d, bool needSplit)
        {
            if (d is null) return string.Empty;
            var convert = (DateTime) d;
            var year = (convert.Year - 1911).ToString().PadLeft(3, '0');
            var month = (convert.Month).ToString().PadLeft(2, '0');
            var day = (convert.Day).ToString().PadLeft(2, '0');
            if (needSplit)
                return year + "/" + month + "/" + day;
            return year + month + day;
        }

        public static DateTime TWDateStringToDateOnly(string date)
        {
            var year = int.Parse(date.Substring(0, 3)) + 1911;
            var month = int.Parse(date.Substring(3, 2));
            var day = int.Parse(date.Substring(5, 2));
            return new DateTime(year,month,day);
        }

        public static DateTime TWDateStringToDateTime(string date)
        {
            var year = int.Parse(date.Substring(0, 3)) + 1911;
            var month = int.Parse(date.Substring(3, 2));
            var day = int.Parse(date.Substring(5, 2));
            var hour = int.Parse(date.Substring(7, 2));
            var min = int.Parse(date.Substring(9, 2));
            var sec = int.Parse(date.Substring(11, 2));
            return new DateTime(year, month, day,hour,min,sec);
        }
        public static string ToStringWithSecond(DateTime d)
        {
            var year = (d.Year - 1911).ToString().PadLeft(3, '0');
            var month = (d.Month).ToString().PadLeft(2, '0');
            var day = (d.Day).ToString().PadLeft(2, '0');
            var hour = (d.Hour).ToString().PadLeft(2, '0');
            var minute = (d.Minute).ToString().PadLeft(2, '0');
            var sec = (d.Second).ToString().PadLeft(2, '0');
            return year + month + day + hour + minute + sec;
        }

        public static string ConvertDateStringSplitToChinese(string date)
        {
            var dateSplit = date.Split('/');
            return dateSplit[0] + "年" + dateSplit[1] + "月" + dateSplit[2] + "日";
        }
        public static bool ValidateDateTime(string datetime, string format)
        {
            if (datetime == null || datetime.Length == 0)
            {
                return false;
            }
            try
            {
                System.Globalization.DateTimeFormatInfo dtfi = new System.Globalization.DateTimeFormatInfo();
                dtfi.FullDateTimePattern = format;
                DateTime dt = DateTime.ParseExact(datetime, "F", dtfi);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static DateTime GetFirstDay(DateTime? declareDate)
        {
            return new DateTime(((DateTime)declareDate).Year, ((DateTime)declareDate).Month, 1);
        }
        public static DateTime GetLastDay(DateTime? declareDate)
        {
            var date = (DateTime) declareDate;
            return new DateTime(date.AddMonths(1).Year, date.AddMonths(1).Month, 1).AddDays(-1);
        }
        public static DateTime GetDateTimeWithDay(DateTime? declareDate,int day)
        {
            var date = (DateTime)declareDate;
            return new DateTime(date.Year, date.Month, day);
        }
    }
}