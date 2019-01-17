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
            //Pro_ID, Pro_ChineseName, Pro_EnglishName, I.Inv_Inventory, ISNULL(I.Inv_SafeAmount, 0) AS Inv_SafeAmount, ISNULL(I.Inv_BasicAmount, 0) AS Inv_BasicAmount, ISNULL(M.Med_Price, 0) AS Med_Price, ISNULL(U.Unit_Price, 0) AS Unit_Price
            ID = row.Field<string>("Pro_ID");
            ChineseName = row.Field<string>("Pro_ChineseName");
            EnglishName = row.Field<string>("Pro_EnglishName");
            Inventory = row.Field<int>("Inv_Inventory");
            SafeAmount = row.Field<int>("Inv_SafeAmount");
            BasicAmount = row.Field<int>("Inv_BasicAmount");
            NHIPrice = (double)row.Field<decimal>("Med_Price");
            SellPrice = (double)row.Field<decimal>("Unit_Price");
            //IsEnable = row.Field<bool>("Is_Enable");
        }

        public string ID { get; set; }
        public string ChineseName { get; set; }
        public string EnglishName { get; set; }
        public string FullName => EnglishName.Substring(0, EnglishName.IndexOf(" ")) + ChineseName;
        public double Inventory { get; set; }
        public int SafeAmount { get; set; }
        public int BasicAmount { get; set; }
        public double NHIPrice { get; set; }
        public double SellPrice { get; set; }
        //public bool IsEnable { get; set; }
    }
}
