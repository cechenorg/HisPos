using System.Collections.Generic;
using His_Pos.Interface;

namespace His_Pos.Class.PaymentCategory
{
    public static class PaymentCategroyDb
    {
        public static List<PaymentCategory> PaymentCategoryList { get; } = new List<PaymentCategory>();

        private static readonly Dictionary<string, string> PaymentCategoryDictionary = new Dictionary<string, string>
        {
            {"1", "職業傷害"}, {"2", "職業病"},
            {"3", "普通傷害"}, {"4", "普通疾病"},
            {"9", "呼吸照護"},{"A", "天然災害 - 巡迴"},
            {"B","天然災害 - 非巡迴"},{"Z","高雄市氣爆事件"}
        };

        public static void GetData()
        {
            foreach (var paymentCategory in PaymentCategoryDictionary)
            {
                var p = new PaymentCategory(paymentCategory.Key,paymentCategory.Value);
                PaymentCategoryList.Add(p);
            }
        }
        /*
         *回傳對應給付類別之id + name string
         */
        public static string GetPaymentCategory(string tag)
        {
            var result = string.Empty;
            foreach (var payment in PaymentCategoryDictionary)
            {
                if (payment.Key == tag)
                {
                    result = payment.Key + ". " + payment.Value;
                }
            }
            return result;
        }
    }
}
