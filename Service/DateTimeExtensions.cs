﻿using System;
using System.Globalization;

namespace His_Pos.Service
{
    public static class DateTimeExtensions
    {

        public static string ConvertToTaiwanCalender(DateTime d,bool needSplit)
        {
            var year = (d.Year - 1911).ToString().PadLeft(3, '0');
            var month = (d.Month).ToString().PadLeft(2, '0');
            var day = (d.Day).ToString().PadLeft(2, '0');
            if (needSplit)
                return year + "/" + month + "/" + day;
            return year + month + day;
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
        public static DateTime GetDateTimeWithDay(DateTime? declareDate,int day)
        {
            var date = (DateTime)declareDate;
            return new DateTime(date.Year, date.Month, day);
        }
    }
}