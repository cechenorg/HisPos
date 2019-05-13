using System.Data;
using GalaSoft.MvvmLight;
using His_Pos.NewClass.Product.Medicine.MedicineSet;

namespace His_Pos.NewClass.Product
{
    public abstract class Product : ObservableObject
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
        }

        public Product(ProductStruct p)
        {
            ID = p.ID;
            ChineseName = p.ChineseName;
            EnglishName = p.EnglishName;
        }
        public Product(MedicineSetItem m)
        {
            ID = m.ID;
            ChineseName = m.ChineseName;
            EnglishName = m.EnglishName;
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
