using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.CustomerHistoryProduct
{
    public struct CustomerHistoryProduct
    {
        public CustomerHistoryProduct(DataRow r)
        {
            ID = r.Field<string>("");
            ChineseName = r.Field<string>("");
            EnglishName = r.Field<string>("");
            Dosage = (double)r.Field<decimal>("");
            UsageName = r.Field<string>("");
            PositionID = r.Field<string>("");
            Days = r.Field<int>("");
            Amount = (double)r.Field<decimal>("");
            TotalPrice = (double)r.Field<decimal>("");
        }
        public string ID { get; set; }
        public string ChineseName { get; set; }
        public string EnglishName { get; set; }
        public string FullName
        {
            get
            {
                if (!string.IsNullOrEmpty(EnglishName))
                    return (EnglishName.Contains(" ") ? EnglishName.Substring(0, EnglishName.IndexOf(" ")) : EnglishName) + ChineseName;
                return !string.IsNullOrEmpty(ChineseName) ? ChineseName : string.Empty;
            }
        }
        public double Dosage { get; set; }
        public string UsageName { get; set; }
        public string PositionID { get; set; }
        public int Days { get; set; }
        public double Amount { get; set; }
        public double TotalPrice { get; set; }
    }
}
