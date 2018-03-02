using System.Collections.Generic;

namespace His_Pos.Class.AdjustCase
{
    public static class AdjustCaseDb
    {
        public static List<AdjustCase> AdjustCaseList { get; } = new List<AdjustCase>();
        private static readonly Dictionary<string, string> AdjustCaseDictionary = new Dictionary<string, string>
        {
            {"1", "一般處方調劑"}, {"2", "慢性病連續處方調劑"},
            {"3", "日劑藥費"}, {"4", "肺結核個案DOTS執行服務費"},
            {"5", "協助辦理門診戒菸計畫"},{"D", "藥事居家照護"}
        };
        public static void GetData()
        {
            foreach (var adjustCase in AdjustCaseDictionary)
            {
                var a = new AdjustCase(adjustCase.Key, adjustCase.Value);
                AdjustCaseList.Add(a);
            }
        }
        /*
         *回傳對應調劑案件之id + name string
         */
        public static string GetAdjustCase(string tag)
        {
            string result = string.Empty;
            foreach (var adjust in AdjustCaseDictionary)
            {
                if (adjust.Key == tag)
                {
                    result = adjust.Key + ". " + adjust.Value;
                }
            }
            return result;
        }
    }
}
