using His_Pos.NewClass.Product.ProductManagement;
using System;
using System.Data;

namespace His_Pos.NewClass.Product
{
    public struct ProductStruct
    {
        public ProductStruct(DataRow row)
        {
            //Type = row.Field<string>("Pro_TypeID").Equals("O") ? ProductTypeEnum.OTCMedicine : ProductTypeEnum.NHIMedicine;

            switch(Convert.ToString(row["Pro_TypeID"]))
            {
                case "O":
                    Type = ProductTypeEnum.OTCMedicine;
                    break;
                case "D":
                    Type = ProductTypeEnum.Deposit;
                    break;
                case "M":
                    Type = ProductTypeEnum.NHIMedicine;
                    break;
                default:
                    Type = ProductTypeEnum.SpecialMedicine;
                    break;
            }

            ID = row.Field<string>("Pro_ID");
            ChineseName = row.Field<string>("Pro_ChineseName");
            EnglishName = row.Field<string>("Pro_EnglishName");
            Inventory = row.Field<double>("Inv_Inventory");
            SafeAmount = row.Field<int>("Inv_SafeAmount");
            BasicAmount = row.Field<int>("Inv_BasicAmount");
            OnTheWayAmount = row.Field<double>("Inv_OnTheWay");
            MedBagOnTheWayAmount = row.Field<double>("Inv_MedBagOnTheWay");
            NHIPrice = (double)row.Field<decimal>("Med_Price");
            SellPrice = row.Field<double>("Unit_Price");
            IsCommon = row.Field<bool>("Pro_IsCommon");
            IsFrozen = row.Field<bool>("Med_IsFrozen");
            ControlLevel = row.Field<byte?>("Med_Control");
            IsEnable = row.Field<bool>("Pro_IsEnable");
            OTCFromSingde = row.Field<bool>("OTCFromSingde");

            /*if (Type == ProductTypeEnum.OTCMedicine)
                SellPrice = 0;*/
        }

        public ProductTypeEnum Type { get; set; }
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

        public double Inventory { get; set; }
        public int SafeAmount { get; set; }
        public int BasicAmount { get; set; }
        public double OnTheWayAmount { get; set; }
        public double MedBagOnTheWayAmount { get; set; }
        public double NHIPrice { get; set; }
        public double SellPrice { get; set; }
        public int? ControlLevel { get; set; }
        public bool IsCommon { get; set; }
        public bool IsFrozen { get; set; }
        public bool IsEnable { get; set; }
        public bool OTCFromSingde { get; set; }
    }
}