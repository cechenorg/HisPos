﻿using System;
using System.Globalization;

namespace His_Pos.Service
{
    public class DateTimeExtensions
    {
        /// <summary>
        /// To the simple taiwan date.
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <returns></returns>
        public string ToSimpleTaiwanDate(DateTime datetime)
        {
            var taiwanCalendar = new TaiwanCalendar();
            var month = datetime.Month.ToString();
            var day = datetime.Day.ToString();
            month = CheckDateLessTen(datetime.Month,month);
            day = CheckDateLessTen(datetime.Day,day);
            return $"{taiwanCalendar.GetYear(datetime)}/{month}/{day}";
        }

        public DateTime ToUsDate(string datetime)
        {
            if (datetime.Length >= 10)
                return Convert.ToDateTime(datetime.Substring(0, 10));
            if (datetime.Substring(0,3).Contains("/"))
                datetime = "0" + datetime;
            var dt = DateTime.ParseExact(datetime, "yyy/MM/dd", CultureInfo.InvariantCulture).AddYears(1911);
            return dt;
        }

        public double CalculateAge(DateTime birthDate)
        {
            var month = birthDate.Month.ToString();
            var day = birthDate.Day.ToString();
            month = CheckDateLessTen(birthDate.Month, month);
            day = CheckDateLessTen(birthDate.Day, day);
            birthDate = DateTime.ParseExact(birthDate.Year+"/"+month+"/"+day, "yyyy/MM/dd", System.Globalization.CultureInfo.InvariantCulture);
            var ts = DateTime.Now - birthDate;
            var age = ts.TotalDays / 365.2422;
            return age;
        }

        public string BirthdayFormatConverter(string birthday)
        {
            var year = birthday.Substring(0, 3).StartsWith("0") ? birthday.Substring(1, 2) : birthday.Substring(0, 3);
            return year + "/" + birthday.Substring(3, 2) + "/" + birthday.Substring(5, 2);
        }
        public string UsToTaiwan(string ustring) {
            string[] split = ustring.Split('/');
            if (split[1].Length == 1) split[1] = "0" + split[1];
            if (split[2].Length == 1) split[2] = "0" + split[2];
            return (Convert.ToInt32(split[0]) - 1911).ToString() + split[1] + split[2];
        }
        private string CheckDateLessTen(int date,string dateStr)
        {
            if(date < 10)
                dateStr = "0" + dateStr;
            return dateStr;
        }
        
    }
}
