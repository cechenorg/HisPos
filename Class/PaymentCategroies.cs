using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using His_Pos.Interface;

namespace His_Pos.Class
{
    public class PaymentCategroies : ISelection
    {
        public List<PaymentCategory> PaymentCategoryList { get; } = new List<PaymentCategory>();

        private readonly Dictionary<string, string> _paymentCategoryDictionary = new Dictionary<string, string>
        {
            {"1", "職業傷害"}, {"2", "職業病"},
            {"3", "普通傷害"}, {"4", "普通疾病"},
            {"9", "呼吸照護"},{"A", "天然災害 - 巡迴"},
            {"B","天然災害 - 非巡迴"},{"Z","高雄市氣爆事件"}
        };

        public void GetData()
        {
            foreach (var paymentCategory in _paymentCategoryDictionary)
            {
                var p = new PaymentCategory(paymentCategory.Key,paymentCategory.Value);
                PaymentCategoryList.Add(p);
            }
        }
    }
}
