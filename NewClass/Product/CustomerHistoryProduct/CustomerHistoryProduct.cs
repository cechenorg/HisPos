using System.Data;

namespace His_Pos.NewClass.Product.CustomerHistoryProduct
{
    public struct CustomerHistoryProduct
    {
        public CustomerHistoryProduct(DataRow r)
        {
            ID = r.Field<string>("Pro_ID");
            ChineseName = r.Field<string>("Pro_ChineseName");
            EnglishName = r.Field<string>("Pro_EnglishName");
            Dosage = r.Field<double?>("Dosage");
            UsageName = r.Field<string>("Usage");
            PositionID = r.Field<string>("Position");
            Days = r.Field<int?>("MedicineDays");
            Amount = r.Field<double>("TotalAmount");
            switch (ID)
            {
                case "R001":
                    ChineseName = "處方箋遺失或毀損，提前回診";
                    return;

                case "R002":
                    ChineseName = "醫師請假，提前回診";
                    return;

                case "R003":
                    ChineseName = "病情變化提前回診，經醫師認定需要改藥或調整藥品劑量或換藥";
                    return;

                case "R004":
                    ChineseName = "其他提前回診或慢箋提前領藥";
                    return;

                case "R005":
                    ChineseName = "新特約無法使用雲端藥歷";
                    break;
            }
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

        public double? Dosage { get; set; }
        public string UsageName { get; set; }
        public string PositionID { get; set; }
        public int? Days { get; set; }
        public double Amount { get; set; }
    }
}