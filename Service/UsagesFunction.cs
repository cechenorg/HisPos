using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using His_Pos.Class;

namespace His_Pos.Service
{
    public static class UsagesFunction
    {
        public static int CheckUsage(Usage usage, int days)
        {
            Regex reg_yWzD = new Regex(@"\d+W\d+D");
            Regex reg_MCDxDy = new Regex(@"MCD\d+D\d+");
            Regex reg_QxD = new Regex(@"Q\d+D");
            Regex reg_QxW = new Regex(@"Q\d+W");
            Regex reg_QxM = new Regex(@"Q\d+M");
            var count = UsagesFunction.CheckStableUsage(usage, days);
            if (count == 0)
            {
                if (usage.Name.StartsWith("QW") || usage.Name.Length > 2)//QW(x,y,z…)：每星期 x，y，z…使用(x,y,z... = 1~7)
                    count = usage.Name.Length - 2;
                else if (reg_yWzD.IsMatch(usage.Name))//每 y 星期使用 z 天(y、z > 0)
                    count = UsagesFunction.yWzD(usage.Name, days);
                else if (reg_MCDxDy.IsMatch(usage.Name))//月經第 x 天至第 y 天使用(x、y > 0,x < y)
                    count = UsagesFunction.MCDxDy(usage.Name);
                else if (reg_QxD.IsMatch(usage.Name))//每 x 日 1 次(x >= 2)
                    count = UsagesFunction.QxD(usage.Name, days);
                else if (reg_QxW.IsMatch(usage.Name))//每 x 星期 1 次(x > 0)
                    count = UsagesFunction.QxW(usage.Name, days);
                else if (reg_QxM.IsMatch(usage.Name))//每 x 月 1 次(x > 0)
                    count = UsagesFunction.QxM(usage.Name, days);
                //else if()
            }
            return count;
        }

        public static int QxM(string str, int days)
        {
            int x = MatchNumber(str);
            if (days % (30 * x) != 0)
                return -1;
            return days / (30 * x);
        }

        public static int QxW(string str, int days)
        {
            int x = MatchNumber(str);
            if (days % (7 * x) != 0)
                return -1;
            return days / (7 * x);
        }

        public static int QxD(string str, int days)
        {
            int x = MatchNumber(str);
            if (days % x != 0)
                return -1;
            return days / x;
        }

        public static int QOD(int days)
        {
            if (days % 2 != 0)
                return -1;
            return days / 2;
        }

        public static int CheckStableUsage(Usage usage, int days)
        {
            int count;
            switch (usage.Name)
            {
                case "QD"://每日 1 次
                case "QDAM"://每日 1 次上午使用
                case "QDPM"://每日 1 次下午使用
                case "QDHS"://每日 1 次睡前使用
                case "QN"://每晚使用 1 次
                case "HS"://睡前 1 次
                    count = days;
                    break;
                case "BID"://每日 2 次
                case "QAM&HS"://上午使用 1 次且睡前 1 次
                case "QPM&HS"://下午使用 1 次且睡前 1 次
                case "QAM&PM"://每日上下午各使用 1 次
                case "BID&HS"://每日 2 次且睡前 1 次
                    count = days * 2;
                    break;
                case "TID"://每日三次
                case "TID&HS"://每日 3 次且睡前 1 次
                    count = days * 3;
                    break;
                case "QID":
                    count = days * 4;
                    break;
                case "STAT"://立刻使用
                case "ASORDER"://依照醫師指示使用
                    count = -1;
                    break;
                case "OQD"://隔日使用 1 次
                    count = QOD(days);
                    break;
                case "QW"://每星期 1 次
                    count = QW(days);
                    break;
                case "BIW"://每星期2次
                    count = BIW(days);
                    break;
                default:
                    count = 0;
                    break;
            }
            return count;
        }

        public static int BIW(int days)
        {
            if (days % 7 != 0)
                return -1;
            return days / 7 * 2;
        }

        public static int QW(int days)
        {
            if (days % 7 != 0)
                return -1;
            return days / 7;
        }

        public static int yWzD(string str, int days)
        {
            int count;
            int[] values = FindNumberInString(str);
            count = days / 7 * values[0] * values[1];
            return count;
        }

        public static int MCDxDy(string str)
        {
            int count;
            int[] values = FindNumberInString(str);
            count = values[1] - values[0] + 1;
            return count;
        }

        public static int[] FindNumberInString(string str)
        {
            int[] values = new int[2];
            int i = 0;
            foreach (Match match in Regex.Matches(str, @"\d+"))
            {
                values[i] = int.Parse(match.Value);
                i++;
            }
            return values;
        }

        public static int MatchNumber(string str)
        {
            Match match = Regex.Match(str, @"\d+");
            return int.Parse(match.Value);
        }
    }
}
