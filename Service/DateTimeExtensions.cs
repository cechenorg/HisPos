using His_Pos.NewClass;
using His_Pos.FunctionWindow;
using System;

namespace His_Pos.Service
{
    public static class DateTimeExtensions
    {
        public static string ConvertToTaiwanCalender(DateTime d)
        {
            var dateStringArr = CreateDateStringFromDateTime(d);
            return dateStringArr[0] + dateStringArr[1] + dateStringArr[2];
        }

        public static string ConvertToTaiwanCalenderWithSplit(DateTime d)
        {
            var dateStringArr = CreateDateStringFromDateTime(d);
            return dateStringArr[0] + "/" + dateStringArr[1] + "/" + dateStringArr[2];
        }

        private static string[] CreateDateStringFromDateTime(DateTime d)
        {
            var year = (d.Year - 1911).ToString().PadLeft(3, '0');
            var month = (d.Month).ToString().PadLeft(2, '0');
            var day = (d.Day).ToString().PadLeft(2, '0');
            return new[] { year, month, day };
        }

        public static string ConvertToTaiwanCalenderWithTime(DateTime d, bool needSplit = false)
        {
            var year = (d.Year - 1911).ToString().PadLeft(3, '0');
            var month = (d.Month).ToString().PadLeft(2, '0');
            var day = (d.Day).ToString().PadLeft(2, '0');
            var hour = (d.Hour).ToString().PadLeft(2, '0');
            var minute = (d.Minute).ToString().PadLeft(2, '0');
            return needSplit ? $"{year}/{month}/{day} {hour}:{minute}" : year + month + day + hour + minute;
        }

        public static string ConvertToTaiwanCalenderWithTimeZero(DateTime d)
        {
            var year = (d.Year - 1911).ToString().PadLeft(3, '0');
            var month = (d.Month).ToString().PadLeft(2, '0');
            var day = (d.Day).ToString().PadLeft(2, '0');
            return year + month + day + "0000";
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
            var convert = (DateTime)d;
            var year = (convert.Year - 1911).ToString().PadLeft(3, '0');
            var month = (convert.Month).ToString().PadLeft(2, '0');
            var day = (convert.Day).ToString().PadLeft(2, '0');
            if (needSplit)
                return year + "/" + month + "/" + day;
            return year + month + day;
        }

        public static DateTime? TWDateStringToDateOnly(string date)
        {
            var dateStr = date.Replace("/", "");
            if (dateStr.Length < 6 || dateStr.Length > 7)
            {
                MessageWindow.ShowMessage("日期格式錯誤，請確認資料", MessageType.ERROR);
                return null;
            }

            if (dateStr.Length == 6)
            {
                dateStr = "0" + dateStr;
            }
            var year = int.Parse(dateStr.Substring(0, 3)) + 1911;
            var month = int.Parse(dateStr.Substring(3, 2));
            var day = int.Parse(dateStr.Substring(5, 2));
            return new DateTime(year, month, day);
        }

        public static DateTime TWDateStringToDateTime(string date)
        {
            var year = int.Parse(date.Substring(0, 3)) + 1911;
            var month = int.Parse(date.Substring(3, 2));
            var day = int.Parse(date.Substring(5, 2));
            var hour = int.Parse(date.Substring(7, 2));
            var min = int.Parse(date.Substring(9, 2));
            var sec = int.Parse(date.Substring(11, 2));
            return new DateTime(year, month, day, hour, min, sec);
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
            if (string.IsNullOrEmpty(datetime)) return false;
            try
            {
                var dateTimeFormatInfo = new System.Globalization.DateTimeFormatInfo { FullDateTimePattern = format };
                var dt = DateTime.ParseExact(datetime, "F", dateTimeFormatInfo);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static DateTime GetDateTimeWithDay(DateTime? declareDate, int day)
        {
            var date = (DateTime)declareDate;
            return new DateTime(date.Year, date.Month, day);
        }

        public static bool CheckIsWeekend(DateTime start)
        {
            return (int)start.DayOfWeek == 0;
        }

        public static int CountTimeDifferenceWithoutHoliday(DateTime startDate, DateTime endDate)
        {
            return new TimeSpan(endDate.Ticks - startDate.Ticks).Days - CountHoliday(startDate, endDate);
        }

        private static int CountHoliday(DateTime start, DateTime end)
        {
            var tmpStartDate = start.DeepCloneViaJson();
            var holiday = 0;
            while (tmpStartDate < end)
            {
                if (CheckIsWeekend(tmpStartDate))
                    holiday += 1;
                tmpStartDate = tmpStartDate.AddDays(1);
            }
            return holiday;
        }

        public static string ConvertDateStringToTaiwanCalendar(string text)
        {
            var dateStr = text.Replace("-", "").Replace("/", "").Trim();
            var dateLength = dateStr.Length;
            if (dateLength < 5) return text;
            var day = int.Parse(dateStr.Substring(dateLength - 2, 2));
            var month = int.Parse(dateStr.Substring(dateLength - 4, 2));
            var year = int.Parse(dateStr.Substring(0, dateLength - 4));
            return year.ToString().PadLeft(3, '0') + "/" + month.ToString().PadLeft(2, '0') + "/" + day.ToString().PadLeft(2, '0');
        }

        public static string ConvertToTaiwanCalendarChineseFormat(DateTime? d, bool needSplit)
        {
            return ConvertDateStringSplitToChinese(NullableDateToTWCalender(d, needSplit));
        }
    }
}