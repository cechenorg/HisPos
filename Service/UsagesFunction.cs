using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using His_Pos.Class;

namespace His_Pos.Service
{
    public static class UsagesFunction
    {
        public static int CheckUsage(Usage usage, int days)
        {
            Regex reg_QWxyz = new Regex(@"QW\d+");
            Regex reg_QWxyzAM = new Regex(@"QW\d+AM");
            Regex reg_yWzD = new Regex(@"\d+W\d+D");
            Regex reg_MCDxDy = new Regex(@"MCD\d+D\d+");
            Regex reg_QxD = new Regex(@"Q\d+D");
            Regex reg_QxW = new Regex(@"Q\d+W[I]*");
            Regex reg_QxM = new Regex(@"Q\d+M");
            Regex reg_QxH = new Regex(@"Q\d+[HI]+");
            Regex reg_QxMN = new Regex(@"Q\d+MN");
            var count = CheckStableUsage(usage, days);
            if (count == -1)
            {
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
                else if (int.Parse(usage.Days) != 0 && int.Parse(usage.Times) != 0)
                {
                    int usageDays = int.Parse(usage.Days);
                    int usageTimes = int.Parse(usage.Times);
                    if (days % usageDays != 0)
                        count = (days / usageDays) * usageTimes + 1;
                    else
                        count = (days / usageDays) * usageTimes;
                }
                else
                    count = 0;
            }
            return count;
        }

        private static int QWxyzAM(string str, int days)
        {
            string replacedStr = str.Replace("AM", "");
            return QWxyz(replacedStr, days);
        }

        private static int QWxyz(string str, int days)
        {
            ObservableCollection<int> values = new ObservableCollection<int>();
            foreach (Match match in Regex.Matches(str, @"\d"))
            {
                values.Add(int.Parse(match.Value));
            }
            if (days % 7 != 0)
            {
                int redundantDays = days % 7;
                for (int i = values.Count - 1; i >= 0; i--)
                {
                    if (days >= values[i])
                        redundantDays = i + 1;
                }

                return days / 7 * (str.Length - 2) + redundantDays;
            }
            return days / 7 * (str.Length - 2);
        }

        private static int QxMN(string str, int days)
        {
            int x = MatchNumber(str);
            if ((days * 24 * 60) % x != 0)
                return days * 24 * 60 / x + 1;
            return days * 24 * 60 / x;
        }

        private static int QxH(string str, int days)
        {
            int x = MatchNumber(str);
            if ((days * 24) % x != 0)
                return days * 24 / x + 1;
            return days * 24 / x;
        }

        private static int QxM(string str, int days)
        {
            int x = MatchNumber(str);
            if (days % (30 * x) != 0)
                return days / (30 * x) + 1;
            return days / (30 * x);
        }

        private static int QxW(string str, int days)
        {
            int x = MatchNumber(str);
            if (days % (7 * x) != 0)
                return -1;
            return days / (7 * x);
        }

        private static int QxD(string str, int days)
        {
            int x = MatchNumber(str);
            if (days % x != 0)
                return -1;
            return days / x;
        }

        private static int QOD(int days)
        {
            if (days % 2 != 0)
                return -1;
            return days / 2;
        }

        private static int CheckStableUsage(Usage usage, int days)
        {
            int count;
            switch (usage.Name)
            {
                case "QD"://每日 1 次
                case "QDA"://每天1次，飯前使用 **
                case "QDI"://每天1次(針劑)
                case "QDAM"://每日 1 次上午使用
                case "QDPM"://每日 1 次下午使用
                case "QDHS"://每日 1 次睡前使用
                case "QDAC"://每天1次，飯前使用
                case "QDPC"://每天1次，飯後服用
                case "QDCC"://早餐用餐時此藥與食物同時服用
                case "QDAMAC"://每天1次，早上飯前使用
                case "QN"://每晚使用 1 次
                case "QM"://每天中午飯後服用
                case "QAM"://每天1次上午使用
                case "QMA"://每天中午飯前服用
                case "HS"://睡前 1 次
                case "HSSUPP"://睡前塞入1次
                case "INAH"://吸入劑，每天1次
                case "BBF"://早餐前30分鐘
                case "BBFI"://早餐前30分鐘(針劑)
                case "BD"://晚餐前30分鐘
                case "BDI"://晚餐前30分鐘(針劑)
                    count = days;
                    break;

                case "BID"://每日 2 次
                case "BIL1"://每天2次，每次(  )ml
                case "BIDA"://每天兩次，早晚飯前 **
                case "QAM&HS"://上午使用 1 次且睡前 1 次
                case "QPM&HS"://下午使用 1 次且睡前 1 次
                case "QAM&PM"://每日上下午各使用 1 次
                case "BID&HS"://每日 2 次且睡前 1 次
                case "BIL"://每天2次，每次( )ml
                case "BIDCC"://早晚用餐時此藥與食物同時服用
                case "BIDAC"://每天2次，早晚飯前
                case "BIDPC"://每天2次，飯後使用
                    count = days * 2;
                    break;

                case "TID"://每日三次
                case "TIDA"://每天3次，三餐飯前  **
                case "TIDAC"://每天3次，飯前使用
                case "TIDI"://每日三次(針劑)
                case "TIDCC"://三餐用餐時此藥與食物同時服用
                case "TIDPC"://每天3次，飯後使用
                case "TID&HS"://每日 3 次且睡前 1 次
                case "BIDACHS"://每天3次，早晚飯前及睡前
                case "BIDPCHS"://每天3次(早晚飯後及睡前)
                case "BIDHS"://每天3次早晚及睡前1次
                    count = days * 3;
                    break;

                case "QID"://每日 4 次 (每天4次，早中晚及睡前) **
                case "QIL"://每天4次，每次(   )ml
                case "QIDA"://每天4次，三餐飯前及睡前 **
                case "QIDACHS"://每天4次，三餐飯前及睡前
                case "QIDAC"://每天4次，三餐飯前及睡前各1次
                    count = days * 4;
                    break;

                case "PID"://每日5次
                    count = days * 5;
                    break;

                case "OQD"://隔日使用 1 次
                case "QODI"://每隔1天1次(針劑)
                case "QON"://隔夜1次
                case "QODAC"://每隔1天使用1次，飯前使用
                    count = QOD(days);
                    break;

                case "QW"://每星期 1 次
                    count = QW(days);
                    break;

                case "BIW"://每星期2次
                case "BIW1"://每週2次(星期二，星期六)
                case "BIW2"://每週2次(星期一，星期五)
                    count = BIW(days);
                    break;

                case "TIW1"://每星期3次(星期一、星期三、星期五)
                case "TIW2"://每星期3次(星期二、星期四、星期六)
                    count = TIW(days);
                    break;

                case "QIW"://每星期4次
                    count = QIW(days);
                    break;

                default:
                    count = -1;
                    break;
            }
            return count;
        }

        private static int TIW(int days)
        {
            if (days % 7 != 0)
                return days / 7 * 3 + 1;
            return days / 7 * 3;
        }

        private static int QIW(int days)
        {
            if (days % 7 != 0)
                return days / 7 * 4 + 1;
            return days / 7 * 4;
        }

        private static int BIW(int days)
        {
            if (days % 7 != 0)
                return days / 7 * 2 + 1;
            return days / 7 * 2;
        }

        private static int QW(int days)
        {
            if (days % 7 != 0)
                return days / 7 + 1;
            return days / 7;
        }

        private static int yWzD(string str, int days)
        {
            int count;
            int[] values = FindNumberInString(str);
            count = days / 7 * values[0] * values[1];
            return count;
        }

        private static int MCDxDy(string str)
        {
            int count;
            int[] values = FindNumberInString(str);
            count = values[1] - values[0] + 1;
            return count;
        }

        private static int[] FindNumberInString(string str)
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

        private static int MatchNumber(string str)
        {
            Match match = Regex.Match(str, @"\d+");
            return int.Parse(match.Value);
        }
    }
}