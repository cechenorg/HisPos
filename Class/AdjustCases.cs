using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class
{
    public class AdjustCases
    {
        public List<AdjustCase> AdjustCaseList { get; } = new List<AdjustCase>();
        private readonly Dictionary<string, string> _adjustCaseDictionary = new Dictionary<string, string>
        {
            {"1", "一般處方調劑"}, {"2", "慢性病連續處方調劑"},
            {"3", "日劑藥費"}, {"4", "肺結核個案DOTS執行服務費"},
            {"5", "協助辦理門診戒菸計畫"},{"D", "藥事居家照護"}
        };
        public void GetData()
        {
            foreach (var adjustCase in _adjustCaseDictionary)
            {
                var a = new AdjustCase(adjustCase.Key, adjustCase.Value);
                AdjustCaseList.Add(a);
            }
        }
    }
}
