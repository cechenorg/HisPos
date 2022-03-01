using His_Pos.NewClass.Medicine.Usage;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace His_Pos.Service
{
    public static class UsagesFunction
    {
        public static int CheckUsage(int days, Usage usage)
        {
            var reg_QWxyz = new Regex(@"QW\d+");
            var reg_yWzD = new Regex(@"\d+W\d+D");
            var reg_MCDxDy = new Regex(@"MCD\d+D\d+");
            var reg_QxD = new Regex(@"Q\d+D");
            var reg_QxW = new Regex(@"Q\d+W[I]*");
            var reg_QxM = new Regex(@"Q\d+M");
            var reg_QxH = new Regex(@"Q\d+[HI]+");
            var reg_QxMN = new Regex(@"Q\d+MN");
            var reg_QWxyzAM = new Regex(@"QW\d+AM");
            int count;
            if (usage is null)
                return 0;
            if (reg_QWxyz.IsMatch(usage.Name)) //QW(x,y,z…)：每星期 x，y，z…使用(x,y,z... = 1~7)
                count = QWxyz(usage.Name, days);
            else if (reg_QWxyzAM.IsMatch(usage.Name)) //QW(x，y，z)AM	每星期x，y，z早上使用
                count = QWxyzAM(usage.Name, days);
            else if (reg_yWzD.IsMatch(usage.Name)) //每 y 星期使用 z 天(y、z > 0)
                count = yWzD(usage.Name, days);
            else if (reg_MCDxDy.IsMatch(usage.Name)) //月經第 x 天至第 y 天使用(x、y > 0,x < y)
                count = MCDxDy(usage.Name);
            else if (reg_QxD.IsMatch(usage.Name)) //每 x 日 1 次(x >= 2)
                count = QxD(usage.Name, days);
            else if (reg_QxW.IsMatch(usage.Name)) //每 x 星期 1 次(x > 0) 每 x 星期 1 次(針劑)(QxW.QxWI)
                count = QxW(usage.Name, days);
            else if (reg_QxM.IsMatch(usage.Name)) //每 x 月 1 次(x > 0)
                count = QxM(usage.Name, days);
            else if (reg_QxH.IsMatch(usage.Name)) //每 x 小時使用 1 次 / 每 x 小時使用1次(針劑)  (QxH.QxI.QxHI)
                count = QxH(usage.Name, days);
            else if (reg_QxMN.IsMatch(usage.Name)) //每 x 分鐘使用 1 次
                count = QxMN(usage.Name, days);
            else if (usage.Days != 0 && usage.Times != 0)
            {
                if (days % usage.Days != 0)
                    count = (days / usage.Days) * usage.Times + 1;
                else
                    count = (days / usage.Days) * usage.Times;
            }
            else
                count = 0;
            return count;
        }

        private static int QWxyzAM(string str, int days)
        {
            var replacedStr = str.Replace("AM", "");
            return QWxyz(replacedStr, days);
        }

        private static int QWxyz(string str, int days)
        {
            var values = FindNumberInString(str);
            var remainDays = days % 7;
            return days / 7 * values.Count + remainDays;
        }

        private static int QxMN(string str, int days)
        {
            var values = FindNumberInString(str);
            if ((days * 24 * 60) % values[0] != 0)
                return days * 24 * 60 / values[0] + 1;
            return days * 24 * 60 / values[0];
        }

        private static int QxH(string str, int days)
        {
            var values = FindNumberInString(str);
            if ((days * 24) % values[0] != 0)
                return days * 24 / values[0] + 1;
            return days * 24 / values[0];
        }

        private static int QxM(string str, int days)
        {
            var values = FindNumberInString(str);
            if (days % (30 * values[0]) != 0)
                return days / (30 * values[0]) + 1;
            return days / (30 * values[0]);
        }

        private static int QxW(string str, int days)
        {
            var values = FindNumberInString(str);
            if (days % (7 * values[0]) != 0)
                return days / (7 * values[0]) + 1;
            return days / (7 * values[0]);
        }

        private static int QxD(string str, int days)
        {
            var values = FindNumberInString(str);
            if (days % values[0] != 0)
                return days / values[0] + 1;
            return days / values[0];
        }

        private static int QOD(int days)
        {
            if (days % 2 != 0)
                return -1;
            return days / 2;
        }

        private static int yWzD(string str, int days)
        {
            var values = FindNumberInString(str);
            return days / 7 * values[0] * values[1]; ;
        }

        private static int MCDxDy(string str)
        {
            var values = FindNumberInString(str);
            return values[1] - values[0];
        }

        private static List<int> FindNumberInString(string str)
        {
            var values = new List<int>();
            foreach (Match match in Regex.Matches(str, @"[0-9][0-9]*"))
                values.Add(int.Parse(match.Value));
            return values;
        }

        public static int GetDaysByUsage_QD(double amount, double dosage)
        {
            var dosagePerDay = dosage;
            return CountDaysByDosagePerDay(amount, dosagePerDay);
        }

        public static int GetDaysByUsage_BID(double amount, double dosage)
        {
            var dosagePerDay = dosage * 2;
            return CountDaysByDosagePerDay(amount, dosagePerDay);
        }

        public static int GetDaysByUsage_TID(double amount, double dosage)
        {
            var dosagePerDay = dosage * 3;
            return CountDaysByDosagePerDay(amount, dosagePerDay);
        }

        public static int GetDaysByUsage_QID(double amount, double dosage)
        {
            var dosagePerDay = dosage * 4;
            return CountDaysByDosagePerDay(amount, dosagePerDay);
        }

        public static int GetDaysByUsage_PID(double amount, double dosage)
        {
            var dosagePerDay = dosage * 5;
            return CountDaysByDosagePerDay(amount, dosagePerDay);
        }

        public static int GetDaysByUsage_QOD(double amount, double dosage)
        {
            var dosagePerDay = dosage * 0.5;
            return CountDaysByDosagePerDay(amount, dosagePerDay);
        }

        private static int CountDaysByDosagePerDay(double amount, double dosagePerDay)
        {
            var days = (int)(amount / dosagePerDay);
            var remain = amount % dosagePerDay;
            return remain == 0 ? days : days + 1;
        }
    }
}