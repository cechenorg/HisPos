using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product
{
    public struct ProductStruct
    {
        public ProductStruct(DataRow row)
        {
            ID = row.Field<string>("");
            ChineseName = row.Field<string>("");
            EnglishName = row.Field<string>("");
            Inventory = row.Field<int>("");
            SafeAmount = row.Field<int>("");
            BasicAmount = row.Field<int>("");
            NHIPrice = row.Field<double>("");
            SellPrice = row.Field<double>("");
        }

        public string ID { get; set; }
        public string ChineseName { get; set; }
        public string EnglishName { get; set; }
        public string FullName
        {
            get { return EnglishName.Substring(0, EnglishName.IndexOf(" ")) + ChineseName; }
        }
        public int Inventory { get; set; }
        public int SafeAmount { get; set; }
        public int BasicAmount { get; set; }
        public double NHIPrice { get; set; }
        public double SellPrice { get; set; }
    }
}
