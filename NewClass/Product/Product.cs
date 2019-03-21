using System.Data;
using GalaSoft.MvvmLight;

namespace His_Pos.NewClass.Product
{
    public class Product : ObservableObject
    {
        public Product()
        {

        }

        public Product(DataRow row)
        {
            ID = row.Field<string>("Pro_ID");
            ChineseName = row.Field<string>("Pro_ChineseName");
            EnglishName = row.Field<string>("Pro_EnglishName");
            IsCommon = row.Field<bool>("Pro_IsCommon");
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
            }
        }

        public Product(ProductStruct p)
        {
            ID = p.ID;
            ChineseName = p.ChineseName;
            EnglishName = p.EnglishName;
        }

        #region ----- Define Variables -----

        private string id;
        public string ID
        {
            get => id;
            set
            {
                Set(() => ID, ref id, value);
            }
        }
        public string ChineseName { get; set; }
        public string EnglishName { get; set; }

        public string FullName
        {
            get
            {
                if(!string.IsNullOrEmpty(EnglishName))
                    return (EnglishName.Contains(" ")? EnglishName.Substring(0, EnglishName.IndexOf(" ")) : EnglishName) + ChineseName;
                return !string.IsNullOrEmpty(ChineseName) ? ChineseName : string.Empty;
            }
        }
        private bool isCommon;//常備品項
        public bool IsCommon
        {
            get => isCommon;
            set
            {
                if (isCommon != value)
                {
                    Set(() => IsCommon, ref isCommon, value);
                }
            }
        }
        #endregion

        #region ----- Define Functions -----


        #endregion
    }
}
